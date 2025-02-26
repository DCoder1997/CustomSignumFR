import * as React from 'react'
import { DateTime } from 'luxon'
import { classes, Dic } from '@framework/Globals'
import { ValueLine, EntityLine, EntityCombo } from '@framework/Lines'
import { FilterOptionParsed } from '@framework/Search'
import { TypeContext } from '@framework/TypeContext'
import * as Finder from '@framework/Finder'
import { Binding, IsByAll, tryGetTypeInfos, TypeReference, getTypeInfos } from '@framework/Reflection'
import { QueryTokenEmbedded, UserAssetMessage } from '../Signum.Entities.UserAssets'
import { QueryFilterEmbedded, PinnedQueryFilterEmbedded } from '../../UserQueries/Signum.Entities.UserQueries'
import { QueryDescription, SubTokensOptions, isFilterGroupOptionParsed, FilterConditionOptionParsed, isList, FilterType, FilterGroupOptionParsed, PinnedFilter, PinnedFilterParsed } from '@framework/FindOptions'
import { Lite, Entity, parseLite, liteKey } from "@framework/Signum.Entities";
import * as Navigator from "@framework/Navigator";
import FilterBuilder, { MultiValue, FilterConditionComponent, FilterGroupComponent, RenderValueContext } from '@framework/SearchControl/FilterBuilder';
import { MList, newMListElement } from '@framework/Signum.Entities';
import { TokenCompleter } from '@framework/Finder';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useForceUpdate, useAPI } from '@framework/Hooks'

interface FilterBuilderEmbeddedProps {
  ctx: TypeContext<MList<QueryFilterEmbedded>>;
  queryKey: string;
  subTokenOptions: SubTokensOptions;
  onChanged?: () => void;
  showPinnedFilterOptions?: boolean
}

export default function FilterBuilderEmbedded(p: FilterBuilderEmbeddedProps) {

  const qd = useAPI(() => Finder.getQueryDescription(p.queryKey), [p.queryKey]);
  const filterOptions = useAPI(() => qd == null ? Promise.resolve(null) : FilterBuilderEmbedded.toFilterOptionParsed(qd, p.ctx.value, p.subTokenOptions), [qd, p.ctx.value, p.subTokenOptions]);

  const forceUpdate = useForceUpdate();
  
  function handleFiltersChanged() {
    var ctx = p.ctx;

    ctx.value.clear();


    function pushFilter(fo: FilterOptionParsed, indent: number) {
      if (isFilterGroupOptionParsed(fo)) {
        ctx.value.push(newMListElement(QueryFilterEmbedded.New({
          isGroup: true,
          indentation: indent,
          groupOperation: fo.groupOperation,
          token: fo.token && QueryTokenEmbedded.New({ token: fo.token, tokenString: fo.token.fullKey }),
          valueString: fo.value,
          pinned: !fo.pinned ? undefined : toPinnedQueryFilterEmbedded(fo.pinned)
        })));

        fo.filters.forEach(f => pushFilter(f, indent + 1));
      } else {

        if (Array.isArray(fo.value) && fo.token) {
          fo.value = fo.value.map(v => v == null || v == "" ? "" :
            fo.token!.filterType == "Embedded" || fo.token!.filterType == "Lite" ? liteKey(v) :
              toStringValue(v, fo.token!.filterType))
            .join("|");
        }

        ctx.value.push(newMListElement(QueryFilterEmbedded.New({
          token: fo.token && QueryTokenEmbedded.New({ token: fo.token, tokenString: fo.token.fullKey }),
          operation: fo.operation,
          valueString: fo.value,
          indentation: indent,
          pinned: !fo.pinned ? undefined : toPinnedQueryFilterEmbedded(fo.pinned)
        })));
      }

      function toPinnedQueryFilterEmbedded(p: PinnedFilter): PinnedQueryFilterEmbedded {
        return PinnedQueryFilterEmbedded.New({
          label: typeof p.label == "function" ? p.label() : p.label,
          column: p.column,
          row: p.row,
          active: p.active,
          splitText: p.splitText,
        });
      }
    }

    filterOptions!.forEach(fo => pushFilter(fo, 0))

    ctx.binding.setValue(ctx.value); //force change

    if (p.onChanged)
      p.onChanged();

    forceUpdate();
  }

  function handleRenderValue(fc: RenderValueContext) {
    if (isFilterGroupOptionParsed(fc.filter)) {

      const f = fc.filter;

      const readOnly = fc.readonly || f.frozen;

      const ctx = new TypeContext<any>(undefined, { formGroupStyle: "None", readOnly: readOnly, formSize: "ExtraSmall" }, undefined as any, Binding.create(f, a => a.value));

      return <ValueLineOrExpression ctx={ctx} onChange={fc.handleValueChange} filterType={"String"} type={{ name: "string" }} />

    } else {

      const f = fc.filter

      const readOnly = fc.readonly || f.frozen;

      const ctx = new TypeContext<any>(undefined, { formGroupStyle: "None", readOnly: readOnly, formSize: "ExtraSmall" }, undefined as any, Binding.create(f, a => a.value));

      if (isList(f.operation!))
        return <MultiLineOrExpression ctx={ctx} onRenderItem={(ctx, onChange) => handleCreateAppropiateControl(ctx, fc, onChange)} onChange={fc.handleValueChange} />;

      return handleCreateAppropiateControl(ctx, fc, () => { });
    }
  }

  function handleCreateAppropiateControl(ctx: TypeContext<any>, fc: RenderValueContext, onChange: () => void) : React.ReactElement<any> {
    const token = fc.filter.token!;

    switch (token.filterType) {
      case "Lite":
      case "Embedded":
        return <EntityLineOrExpression ctx={ctx} onChange={() => { onChange(); fc.handleValueChange(); }} filterType={token.filterType} type={token.type} />;
      default:
        return <ValueLineOrExpression ctx={ctx} onChange={() => { onChange(); fc.handleValueChange(); }} filterType={token.filterType} type={token.type} />

    }
  }
  return (
    <div>
      {
        qd != null &&
        <FilterBuilder
          title={p.ctx.niceName()}
          queryDescription={qd}
          filterOptions={filterOptions ?? []}
          subTokensOptions={p.subTokenOptions}
          readOnly={p.ctx.readOnly}
          onFiltersChanged={handleFiltersChanged}
          showPinnedFiltersOptions={p.showPinnedFilterOptions}
          showPinnedFiltersOptionsButton={false}
          renderValue={handleRenderValue} />
      }
    </div>
  );
}

FilterBuilderEmbedded.toFilterOptionParsed = async function toFilterOptionParsed(qd: QueryDescription, allFilters: MList<QueryFilterEmbedded>, subTokenOptions: SubTokensOptions): Promise<FilterOptionParsed[]> {
  const completer = new TokenCompleter(qd);

  allFilters.forEach(mle => {
    if (mle.element.token && mle.element.token.tokenString)
      completer.request(mle.element.token.tokenString, subTokenOptions);
  });

  await completer.finished();

  function toFilterList(filters: QueryFilterEmbedded[], indent: number): FilterOptionParsed[] {
    return filters.groupWhen(f => f.indentation == indent).map(gr => {
      if (!gr.key.isGroup) {
        if (gr.elements.length != 0)
          throw new Error("Unexpected childrens of condition");

        const pinned = gr.key.pinned;

        const filterCondition: FilterConditionOptionParsed = {
          token: completer.get(gr.key.token!.tokenString),
          operation: gr.key.operation ?? "EqualTo",
          value: gr.key.valueString,
          frozen: false,
          pinned: !pinned ? undefined : toPinnedFilterParsed(pinned),
        };

        return filterCondition;
      }
      else {

        const pinned = gr.key.pinned;

        const filterGroup: FilterGroupOptionParsed = {
          token: gr.key.token ? completer.get(gr.key.token.tokenString) : undefined,
          groupOperation: gr.key.groupOperation!,
          filters: toFilterList(gr.elements, indent + 1),
          value: gr.key.valueString ?? undefined,
          frozen: false,
          expanded: false,
          pinned: !pinned ? undefined : toPinnedFilterParsed(pinned),
        };

        return filterGroup;
      }
    });

    function toPinnedFilterParsed(pinned: PinnedQueryFilterEmbedded): PinnedFilterParsed {
      return {
        label: pinned.label ||  undefined,
        column: pinned.column ?? undefined,
        row: pinned.row ?? undefined,
        active: pinned.active || undefined,
        splitText: pinned.splitText || undefined,
      };
    }
  }

  return toFilterList(allFilters.map(a => a.element), 0);
}

interface MultiLineOrExpressionProps {
  ctx: TypeContext<string | null | undefined>;
  onChange: () => void;
  onRenderItem: (ctx: TypeContext<any>, onChange: () => void) => React.ReactElement<any>;
}

export function MultiLineOrExpression(p: MultiLineOrExpressionProps) {

  const [values, setValues] = React.useState<string[]>([]);

  React.useEffect(() => {
    setValues((p.ctx.value ?? "").split("|"));
  }, [p.ctx.value]);

  const handleChangeValue = () => {
    p.ctx.value = values.join("|");
    if (p.onChange)
      p.onChange();
  }

  return <MultiValue values={values} onChange={handleChangeValue} readOnly={p.ctx.readOnly} onRenderItem={ctx => p.onRenderItem(ctx, handleChangeValue)} />;
}

interface EntityLineOrExpressionProps {
  ctx: TypeContext<string | null | undefined>;
  onChange: () => void;
  type: TypeReference;
  filterType: FilterType;
}

export function EntityLineOrExpression(p: EntityLineOrExpressionProps) {

  const forceUpdate = useForceUpdate();

  const liteRef = React.useRef<Lite<Entity> | null | undefined>(undefined);

  React.useEffect(() => {
    var lite = p.ctx.value == null ? null :
      p.ctx.value.contains(";") ? parseLite(p.ctx.value) :
        undefined;

    liteRef.current = lite;

    if (lite != null) {
      Navigator.API.fillToStrings(lite)
        .then(() => forceUpdate())
        .done();
    }
  }, [p.ctx.value]);

  function getSwitchModelButton(isValue: boolean): React.ReactElement<any> {
    return (<a href="#" className={classes("sf-line-button", "sf-remove", "btn input-group-text", p.ctx.readOnly  && "disabled")}
      onClick={e => { e.preventDefault(); liteRef.current = isValue ? undefined : null; forceUpdate() }}
      title={isValue ? UserAssetMessage.SwitchToExpression.niceToString() : UserAssetMessage.SwitchToValue.niceToString()}>
      <FontAwesomeIcon icon={[isValue ? "far" : "fas", "edit"]} />
    </a>)
  }

  if (liteRef.current === undefined)
    return <ValueLine ctx={p.ctx} type={{ name: "string" }} onChange={p.onChange} extraButtons={() => getSwitchModelButton(false)} />;

  const ctx = new TypeContext<any>(undefined, { formGroupStyle: "None", readOnly: p.ctx.readOnly, formSize: "ExtraSmall" }, undefined as any, Binding.create(liteRef, a => a.current));

  const handleChangeValue = () => {
    p.ctx.value = ctx.value ? liteKey(ctx.value) : null;
    if (p.onChange)
      p.onChange();
  }

  const type = p.type;

  if (p.filterType == "Lite") {
    if (type.name == IsByAll || getTypeInfos(type).some(ti => !ti.isLowPopulation))
      return <EntityLine ctx={ctx} type={type} create={false} onChange={handleChangeValue} extraButtonsAfter={() => getSwitchModelButton(true)} />;
    else
      return <EntityCombo ctx={ctx} type={type} create={false} onChange={handleChangeValue} extraButtonsAfter={() => getSwitchModelButton(true)} />;
  }
  else if (p.filterType == "Embedded") {
    return <EntityLine ctx={ctx} type={type} create={false} autocomplete={null} onChange={handleChangeValue} extraButtonsAfter={() => getSwitchModelButton(true)} />;
  }
  else
    throw new Error("Unexpected Filter Type");
}


interface ValueLineOrExpressionProps {
  ctx: TypeContext<string | null | undefined>;
  onChange: () => void;
  type: TypeReference;
  formatText?: string;
  unitText?: string;
  filterType?: FilterType;
}

export function ValueLineOrExpression(p: ValueLineOrExpressionProps) {

  const foceUpdate = useForceUpdate();
  const valueRef = React.useRef<string | number | boolean | null | undefined>(undefined);

  React.useEffect(() => {
    valueRef.current = parseValue(p.ctx.value, p.filterType)
  }, [p.ctx.value, p.filterType]);


  function getSwitchModelButton(isValue: boolean) : React.ReactElement<any> {
    return (
      <a href="#" className={classes("sf-line-button", "sf-remove", "btn input-group-text")}
        onClick={e => {
          e.preventDefault();
          if (p.filterType == "DateTime")
            p.ctx.value = "yyyy/mm/dd hh:mm:ss";

          if (p.filterType == "Lite")
            p.ctx.value = "[CurrentEntity]";

          valueRef.current = isValue ? undefined : null;
          foceUpdate();
        }}
        title={isValue ? UserAssetMessage.SwitchToExpression.niceToString() : UserAssetMessage.SwitchToValue.niceToString()}>
        <FontAwesomeIcon icon={[isValue ? "far" : "fas", "edit"]} />
      </a>
    );
  }
  if (valueRef.current === undefined)
    return <ValueLine ctx={p.ctx} type={{ name: "string" }} onChange={p.onChange} extraButtons={() => getSwitchModelButton(false)} />;

  const ctx = new TypeContext<any>(undefined, { formGroupStyle: "None", readOnly: p.ctx.readOnly, formSize: "ExtraSmall" }, undefined as any, Binding.create(valueRef, a => a.current));

  const handleChangeValue = () => {
    p.ctx.value = toStringValue(ctx.value, p.filterType);
    if (p.onChange)
      p.onChange();
  }

  const type = p.type;

  if (p.filterType == "Enum") {
    const ti = tryGetTypeInfos(type).single();
    if (!ti)
      throw new Error(`EnumType ${type.name} not found`);
    const members = Dic.getValues(ti.members).filter(a => !a.isIgnoredEnum);
    return <ValueLine ctx={ctx} type={type} formatText={p.formatText} unitText={p.unitText} onChange={handleChangeValue} extraButtons={() => getSwitchModelButton(true)} optionItems={members} />;
  } else {
    return <ValueLine ctx={ctx} type={type} formatText={p.formatText} unitText={p.unitText} onChange={handleChangeValue} extraButtons={() => getSwitchModelButton(true)} />;
  }
}

const serverDateTimeFormat = "yyyy/MM/dd HH:mm:ss";
const serverTimeFormat = "HH:mm:ss.u";

function parseValue(str: string | null | undefined, filterType: FilterType | undefined): string | number | boolean | null | undefined {
  return str == null ? null :
    filterType == "Integer" ? parseInt(str) :
      filterType == "Decimal" ? parseFloat(str) :
        filterType == "Boolean" ? (str == "True" ? true : str == "False" ? false : undefined) :
          filterType == "DateTime" ? parseDate(str) :
            filterType == "Time" ? parseTime(str) :
              filterType == "Enum" || filterType == "Guid" || filterType == "String" ? str :
                undefined;
}

function parseDate(str: string) {
  const parsed = DateTime.fromFormat(str, serverDateTimeFormat).toISO();

  return parsed ?? undefined;
}

function parseTime(str: string) {
  const parsed = DateTime.fromFormat(str, serverTimeFormat).toFormat(serverTimeFormat);

  return parsed ?? undefined;
}

function toStringValue(value: string | number | boolean | null | undefined, filterType: FilterType | undefined): string | null {
  return value == null ? null :
    filterType == "Integer" ? value.toString() :
      filterType == "Decimal" ? value.toString() :
        filterType == "Boolean" ? (value ? "True" : "False") :
          filterType == "DateTime" ? DateTime.fromISO(value as string).toFormat(serverDateTimeFormat) :
            filterType == "Time" ? DateTime.fromFormat(value as string, serverTimeFormat).toFormat(serverTimeFormat) :
              filterType == "Enum" || filterType == "Guid" || filterType == "String" ? value as string :
                null;
}
