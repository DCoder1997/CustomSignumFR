import * as React from 'react'
import { ModifiableEntity, Lite, Entity, toLite, is, liteKey, getToString } from '../Signum.Entities'
import * as Finder from '../Finder'
import { FindOptions } from '../FindOptions'
import { TypeContext } from '../TypeContext'
import { TypeReference } from '../Reflection'
import { EntityBase, EntityBaseProps } from './EntityBase'
import { FormGroup } from './FormGroup'
import { FormControlReadonly } from './FormControlReadonly'
import { classes } from '../Globals';


export interface EntityComboProps extends EntityBaseProps {
  ctx: TypeContext<ModifiableEntity | Lite<Entity> | null | undefined>;
  data?: Lite<Entity>[];
  labelTextWithData?: (data: Lite<Entity>[] | undefined | null) => React.ReactChild;
  refreshKey?: string;
}

export class EntityCombo extends EntityBase<EntityComboProps, EntityComboProps> {

  calculateDefaultState(state: EntityComboProps) {
    state.remove = false;
    state.create = false;
    state.view = false;
    state.viewOnCreate = true;
    state.find = false;
  }

  entityComboSelect?: EntityComboSelect | null;

  renderInternal() {
    const s = this.state;

    const hasValue = !!s.ctx.value;

    const buttons = (
      <span className="input-group-append">
        {!hasValue && this.renderCreateButton(true)}
        {!hasValue && this.renderFindButton(true)}
        {hasValue && this.renderViewButton(true, this.state.ctx.value!)}
        {hasValue && this.renderRemoveButton(true, this.state.ctx.value!)}
        {this.props.extraButtons && this.props.extraButtons(this)}
      </span>
    );

    return (
      <FormGroup ctx={s.ctx}
        labelText={s.labelTextWithData == null ? s.labelText : s.labelTextWithData(this.props.data || this.entityComboSelect && this.entityComboSelect.state.data)}
        helpText={s.helpText}
        htmlAttributes={{ ...this.baseHtmlAttributes(), ...EntityBase.entityHtmlAttributes(s.ctx.value), ...s.formGroupHtmlAttributes }}
        labelHtmlAttributes={s.labelHtmlAttributes} >
        <div className="SF-entity-combo">
          <div className={EntityBase.hasChildrens(buttons) ? s.ctx.inputGroupClass : undefined}>
            <EntityComboSelect
              ref={ecs => this.entityComboSelect = ecs}
              ctx={s.ctx}
              onChange={this.handleOnChange}
              type={s.type!}
              data={s.data}
              findOptions={s.findOptions}
              onDataLoaded={s.labelTextWithData == null ? undefined : () => this.forceUpdate()}
              mandatoryClass={this.mandatoryClass}
              refreshKey={s.refreshKey}
            />
            {EntityBase.hasChildrens(buttons) ? buttons : undefined}
          </div>
        </div>
      </FormGroup>
    );
  }

  doView(entity: ModifiableEntity | Lite<Entity>) {
    var promise = super.doView(entity);

    if (this.props.refreshKey == null) {
      if (promise == null)
        this.setState({ refreshKey: new Date().getTime().toString() });
      else
        promise = promise.then(a => {
          this.setState({ refreshKey: new Date().getTime().toString() });
          return a;
        });
    }

    return promise;
  }

  handleOnChange = (lite: Lite<Entity> | null) => {
    if (lite == null)
      this.setValue(lite);
    else
      this.convert(lite)
        .then(v => this.setValue(v))
        .done();
  }
}

export interface EntityComboSelectProps {
  ctx: TypeContext<ModifiableEntity | Lite<Entity> | null | undefined>;
  onChange: (lite: Lite<Entity> | null) => void;
  type: TypeReference;
  findOptions?: FindOptions;
  data?: Lite<Entity>[];
  mandatoryClass: string | null; 
  onDataLoaded?: (data: Lite<Entity>[] | undefined) => void;
  refreshKey?: string;
}

//Extracted to another component
class EntityComboSelect extends React.Component<EntityComboSelectProps, { data?: Lite<Entity>[] }>{

  constructor(props: EntityComboSelectProps) {
    super(props);
    this.state = { data: props.data };
  }

  componentWillMount() {
    if (this.state.data == null)
      this.reloadData(this.props);
  }

  componentWillReceiveProps(newProps: EntityComboSelectProps, newContext: any) {
    if (newProps.data) {
      if (this.props.data == null)
        console.warn(`The 'data' was set too late. Consider using [] as default value to avoid automatic query. EntityCombo: ${this.props.type!.name}`);

      this.setState({ data: newProps.data });
    } else {
      if (EntityComboSelect.getFindOptions(newProps.findOptions) != EntityComboSelect.getFindOptions(this.props.findOptions) ||
        newProps.type.name != this.props.type.name ||
        newProps.refreshKey != this.props.refreshKey)
        this.reloadData(newProps);
    }
  }

  static getFindOptions(fo: FindOptions | undefined) {
    if (fo == undefined)
      return undefined;

    return Finder.findOptionsPath(fo);
  }

  render() {

    const lite = this.getLite();

    const ctx = this.props.ctx;

    if (ctx.readOnly)
      return <FormControlReadonly ctx={ctx}>{ctx.value && getToString(ctx.value)}</FormControlReadonly>;

    return (
      <select className={classes(ctx.formControlClass, this.props.mandatoryClass)} onChange={this.handleOnChange} value={lite ? liteKey(lite) : ""} disabled={ctx.readOnly} >
        {this.renderOptions()}
      </select>
    );
  }

  handleOnChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    const current = event.currentTarget as HTMLSelectElement;

    if (current.value != this.getLiteKey()) {
      if (!current.value) {
        this.props.onChange(null);
      } else {
        const lite = this.state.data!.filter(a => liteKey(a) == current.value).single();

        this.props.onChange(lite);
      }
    }
  }

  getLite() {
    const v = this.props.ctx.value;
    if (v == undefined)
      return undefined;

    if ((v as Entity).Type)
      return toLite(v as Entity);

    return v as Lite<Entity>;
  }

  getLiteKey() {
    const lite = this.getLite();

    return lite ? liteKey(lite) : undefined;
  }

  renderOptions() {

    if (this.state.data == undefined)
      return undefined;

    const lite = this.getLite();

    const elements = [undefined, ...this.state.data];
    if (lite && !elements.some(a => is(a, lite)))
      elements.insertAt(1, lite);

    return (
      elements.map((e, i) => <option key={i} value={e ? liteKey(e) : ""}>{e ? getToString(e) : " - "}</option>)
    );
  }

  reloadData(props: EntityComboSelectProps) {
    const fo = props.findOptions;
    if (fo) {
      Finder.expandParentColumn(fo);
      var limit = fo && fo.pagination && fo.pagination.elementsPerPage || 999;
      Finder.fetchEntitiesWithFilters(fo.queryName, fo.filterOptions || [], fo.orderOptions || [], limit)
        .then(data => this.setData(fo.orderOptions && fo.orderOptions.length ? data : data.orderBy(a => a.toStr)))
        .done();
    }
    else
      Finder.API.fetchAllLites({ types: this.props.type!.name })
        .then(data => this.setData(data.orderBy(a => a)))
        .done();
  }

  setData(data: Lite<Entity>[]) {
    this.setState({ data: data });
    if (this.props.onDataLoaded)
      this.props.onDataLoaded(data);
  }
}

