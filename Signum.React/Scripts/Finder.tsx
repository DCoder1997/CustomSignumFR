﻿import * as React from "react"
import * as moment from "moment"
import { Router, Route, Redirect, IndexRoute } from "react-router"
import { Dic } from './Globals'
import { ajaxGet, ajaxPost } from './Services';

import { QueryDescription, QueryRequest, FindOptions, FilterOption, FilterType, FilterOperation,
QueryToken, ColumnDescription, ColumnOptionsMode, ColumnOption, Pagination, PaginationMode, ResultColumn,
ResultTable, ResultRow, OrderOption, OrderType, SubTokensOptions, toQueryToken } from './FindOptions';

import { Entity, IEntity, Lite, toLite, liteKey, parseLite, EntityControlMessage  } from './Signum.Entities';

import { Type, IType, EntityKind, QueryKey, getQueryNiceName, getQueryKey, TypeReference,
getTypeInfo, getTypeInfos, getEnumInfo, toMomentFormat } from './Reflection';

import {navigateRoute, isNavigable, currentHistory } from './Navigator';
import SearchPopup from './SearchControl/SearchPopup';
import { Link  } from 'react-router';


export const querySettings: { [queryKey: string]: QuerySettings } = {};

export function start(options: { routes: JSX.Element[] }) {
    options.routes.push(<Route path="find">
        <Route path=":queryName" getComponent={ (loc, cb) => require(["./SearchControl/SearchPage"], (Comp) => cb(null, Comp.default)) } />
        </Route>);
}

export function addSettings(...settings: QuerySettings[]) {
    settings.forEach(s=> Dic.addOrThrow(querySettings, getQueryKey(s.queryName), s));
}

export function getQuerySettings(queryName: any): QuerySettings {
    return querySettings[getQueryKey(queryName)];
}




export const isFindableEvent: Array<(queryKey: string) => boolean> = [];

export function isFindable(queryName: any): boolean {

    const queryKey = getQueryKey(queryName);

    return isFindableEvent.every(f=> f(queryKey));
}

export function find<T extends Entity>(type: Type<T>): Promise<Lite<T>>;
export function find(findOptions: FindOptions): Promise<Lite<IEntity>>;
export function find(findOptions: FindOptions | Type<any> ): Promise<Lite<IEntity>> {

    const fo = (findOptions as FindOptions).queryName ? findOptions as FindOptions :
        { queryName: findOptions } as FindOptions;
    
    return new Promise<Lite<IEntity>>((resolve) => {
        require(["./SearchControl/SearchPopup"], function (SP: { default: typeof SearchPopup }) {
            SP.default.open(fo).then(resolve);
        });
    });
}

export function findMany<T extends Entity>(type: Type<T>): Promise<Lite<T>[]>;
export function findMany(findOptions: FindOptions): Promise<Lite<IEntity>[]>;
export function findMany(findOptions: FindOptions | Type<any>): Promise<Lite<IEntity>[]> {

    const fo = (findOptions as FindOptions).queryName ? findOptions as FindOptions :
        { queryName: findOptions } as FindOptions;

    return new Promise<Lite<IEntity>[]>((resolve) => {
        require(["./SearchControl/SearchPopup"], function (SP: { default: typeof SearchPopup }) {
            SP.default.openMany(fo).then(resolve);
        });
    });
}

export function findOptionsPath(findOptions: FindOptions): string;
export function findOptionsPath(queryName: any): string
{
    const fo = queryName as FindOptions;
    if (!fo.queryName)
        return currentHistory.createPath("/Find/" + getQueryKey(queryName)); 
    
    const base = findOptionsPath(fo.queryName);

    const query = {
        filters: Encoder.encodeFilters(fo.filterOptions),
        orders: Encoder.encodeOrders(fo.orderOptions),
        columns: Encoder.encodeColumns(fo.columnOptions),
        columnOptions: !fo.columnOptionsMode || fo.columnOptionsMode == ColumnOptionsMode.Add ? null : ColumnOptionsMode[fo.columnOptionsMode],
        create: fo.create,
        navigate: fo.navigate,
        searchOnLoad: fo.searchOnLoad,
        showFilterButton: fo.showFilterButton,
        showFilters: fo.showFilters,
        showFooter: fo.showFooter,
        showHeader: fo.showHeader,
        allowChangeColumns: fo.allowChangeColumns,
    };

    return currentHistory.createPath("/Find/" + getQueryKey(fo.queryName), query);
}

export function parseFindOptionsPath(queryName: string, query: any): FindOptions {
    
    const result = {
        queryName: queryName,
        filterOptions: Decoder.decodeFilters(query.filters),
        orderOptions: Decoder.decodeOrders(query.orders),
        columnOptions: Decoder.decodeColumns(query.columns),
        columnOptionsMode: query.columnOptions,
        create: parseBoolean(query.create),
        navigate: parseBoolean(query.navigate),
        searchOnLoad: parseBoolean(query.searchOnLoad),
        showFilterButton: parseBoolean(query.showFilterButton),
        showFilters: parseBoolean(query.showFilters),
        showFooter: parseBoolean(query.showFooter),
        showHeader: parseBoolean(query.showHeader),
    };

    return result;
}




export function parseTokens(findOptions: FindOptions): Promise<FindOptions> {

    const completer = new TokenCompleter(findOptions.queryName);

    if (findOptions.filterOptions)
        findOptions.filterOptions.forEach(fo=> completer.complete(fo, SubTokensOptions.CanElement | SubTokensOptions.CanAnyAll).then(_=> parseValue(fo)));

    if (findOptions.orderOptions)
        findOptions.orderOptions.forEach(fo=> completer.complete(fo, SubTokensOptions.CanElement));

    if (findOptions.columnOptions)
        findOptions.columnOptions.forEach(fo=> completer.complete(fo, SubTokensOptions.CanElement));

    return completer.finish().then(a=> findOptions);
}

class TokenCompleter {
    constructor(public queryName: any) { }

    tokensToRequest: { [fullKey: string]: ({ options: SubTokensOptions, promise: Promise<QueryToken>, resolve: (action: QueryToken) => void }) };


    complete(tokenContainer: { columnName: string, token?: QueryToken }, options: SubTokensOptions): Promise<void> {
        if (tokenContainer.token)
            return;

        return this.request(tokenContainer.columnName, options)
            .then(token => { tokenContainer.token = token; });
    }


    request(fullKey: string, options: SubTokensOptions): Promise<QueryToken> {

        if (!fullKey.contains("."))
            return API.getQueryDescription(this.queryName).then(qd=> toQueryToken(qd.columns[fullKey]));

        var bucket = this.tokensToRequest[fullKey];

        if (bucket)
            return bucket.promise;

        bucket = { promise: null, resolve: null, options: options };

        bucket.promise = new Promise<QueryToken>((resolve, reject) => {
            bucket.resolve = resolve;
        });

        return bucket.promise;
    }


    finish(): Promise<void> {
        const queryKey = getQueryKey(this.queryName);
        const tokens = Dic.map(this.tokensToRequest, (token, val) => ({ token: token, options: val.options }));
        
        if (tokens.length == 0)
            return Promise.resolve(null);
        
        return API.parseTokens(queryKey, tokens).then(parsedTokens=> {
            parsedTokens.forEach(t=> this.tokensToRequest[t.fullKey].resolve(t));
        });
    }
}

function parseValue(fo: FilterOption) {
    switch (filterType(fo.token)) {
        case FilterType.Boolean: fo.value = parseBoolean(fo.value);
        case FilterType.Integer: fo.value = parseInt(fo.value) || null;
        case FilterType.Decimal: fo.value = parseFloat(fo.value) || null;
        case FilterType.Lite:
            {
                if (typeof fo.value == "string")
                    fo.value = parseLite(fo.value);
            }
    }
}

function filterType(queryToken: QueryToken) {
    if ((queryToken as any).filterType)
        return (queryToken as any).filterType;

    else (queryToken as any).filterType = calculateFilterType(queryToken.type);
}


function calculateFilterType(typeRef: TypeReference): FilterType {
    
    if (typeRef.name == "boolean")
        return FilterType.Boolean;

    return FilterType.Boolean;
}


export module API {

    const queryDescriptionCache: { [queryKey: string]: QueryDescription } = {};

    export function getQueryDescription(queryName: any): Promise<QueryDescription> {

        const key = getQueryKey(queryName);

        if (queryDescriptionCache[key])
            return Promise.resolve(queryDescriptionCache[key]);

        return ajaxGet<QueryDescription>({ url: "/api/query/description/" + key })
            .then(qd => {
                queryDescriptionCache[key] = qd;
                return qd;
            });
    }


    export function search(request: QueryRequest): Promise<ResultTable> {
        return ajaxPost<ResultTable>({ url: "/api/query/search" }, request);
    }

    export function findLiteLike(request: { types: string, subString: string, count: number }): Promise<Lite<IEntity>[]> {
        return ajaxGet<Lite<IEntity>[]>({
            url: currentHistory.createHref("api/query/findLiteLike", request)
        });
    }

    export function findAllLites(request: { types: string }): Promise<Lite<IEntity>[]> {
        return ajaxGet<Lite<IEntity>[]>({
            url: currentHistory.createHref("api/query/findAllLites", request)
        });
    }

    export function parseTokens(queryKey: string, tokens: { token: string, options: SubTokensOptions }[]): Promise<QueryToken[]> {
        return ajaxPost<QueryToken[]>({ url: "/api/query/parseTokens" }, { queryKey, tokens });
    }

    export function subTokens(queryKey: string, token: QueryToken, options: SubTokensOptions): Promise<QueryToken[]>{
        return ajaxPost<QueryToken[]>({ url: "/api/query/subTokens" }, { queryKey, token: token == null ? null:  token.fullKey, options }).then(list=> {
            list.forEach(t=> t.parent = token);
            return list;
        });
    }
}

function parseBoolean(value: any): boolean
{
    if (value === "true" || value === true)
        return true;


    if (value === "false" || value === false)
        return false;

    return undefined;
}

module Encoder {

    export function encodeFilters(filterOptions: FilterOption[]) {
        return !filterOptions ? null : filterOptions.map(fo=> getTokenString(fo) + "," + FilterOperation[fo.operation] + "," + stringValue(fo.value));
    }

    export function encodeOrders(orderOptions: OrderOption[]) {
        return !orderOptions ? null : orderOptions.map(oo=> (oo.orderType == OrderType.Descending ? "-" : "") + getTokenString(oo));
    }

    export function encodeColumns(columnOptions: ColumnOption[]) {
        return !columnOptions ? null : columnOptions.map(co=> getTokenString(co) + (co.displayName ? ("," + co.displayName) : ""));
    }

    export function stringValue(value: any): string {

        if (!value)
            return value;

        if (value.Type)
            value = toLite(value as IEntity);

        if (value.EntityType)
            return liteKey(value as Lite<IEntity>);

        return value.toString();
    }
}

module Decoder {

    export function asArray(queryPosition: string | string[]) {

        if (typeof queryPosition == "string")
            return [queryPosition as string];

        return queryPosition as string[]
    }


    export function decodeFilters(filters: string | string[]): FilterOption[] {

        if (!filters)
            return undefined;
        
        return asArray(filters).map(val=> val.split(","))
            .map(vals=> ({
                columnName: vals[0],
                operation: vals[1] as any,
                value: vals[2]
            }) as FilterOption);
    }

    export function decodeOrders(orders: string | string[]): OrderOption[] {
        
        if (!orders)
            return undefined;

        return asArray(orders).map(val=> ({
            orderType: val[0] == "-" ? OrderType.Descending : OrderType.Ascending,
            columnName: val.tryAfter("-") || val
        }));
    }

    export function decodeColumns(columns: string | string[]): ColumnOption[]{

        if (!columns)
            return undefined;

        return asArray(columns).map(val=> ({
            columnName: val[0].tryBefore(",") || val[0],
            displayName: val[0].tryAfter(",")
        }) as ColumnOption);
    }

    export function getEntity(value: any): string {

        if (!value)
            return value;

        if (value.Type)
            value = toLite(value as IEntity);

        if (value.EntityType)
            return liteKey(value as Lite<IEntity>);

        return value.toString();
    }
}

function getTokenString(tokenContainer: { columnName: string, token?: QueryToken }) {
    return tokenContainer.token ? tokenContainer.token.fullKey : tokenContainer.columnName;
}


export module ButtonBarQuery {

    export function getContextBarElements(queryKey: string) {
        return null;
    }

}


export const defaultPagination: Pagination = {
    mode: PaginationMode.Paginate,
    elementsPerPage: 20,
    currentPage: 1,
};


export const defaultOrderColumn: string = "Id";

export interface QuerySettings {
    queryName: any;
    pagination?: Pagination;
    defaultOrderColumn?: string;
    formatters?: { [columnName: string]: CellFormatter };
    rowAttributes?: (row: ResultRow, columns: string[]) => React.HTMLAttributes;
    entityFormatter?: EntityFormatter;
}

export interface FormatRule {
    name: string;
    formatter: (column: ColumnOption) => CellFormatter;
    isApplicable: (column: ColumnOption) => boolean;
}

export class CellFormatter {
    constructor(
        public formatter: (cell: any) => React.ReactNode,
        public textAllign = "left") {
    }
}


export const formatRules: FormatRule[] = [
    {
        name: "Object",
        isApplicable: col=> true,
        formatter: col=> new CellFormatter(cell => cell ? (cell.toStr || cell.toString()) : null)
    },
    {
        name: "Enum",
        isApplicable: col=> col.token.filterType == FilterType.Enum,
        formatter: col=> new CellFormatter(cell => getEnumInfo(col.token.type.name, cell).niceName)
    },
    {
        name: "Lite",
        isApplicable: col=> col.token.filterType == FilterType.Lite,
        formatter: col=> new CellFormatter((cell: Lite<IEntity>) => !cell ? null :
            isNavigable((cell as Lite<any>).EntityType, null) ? <Link to={navigateRoute(cell) }>{cell.toStr}</Link> : cell.toStr)
    },

    {
        name: "Guid",
        isApplicable: col=> col.token.filterType == FilterType.Guid,
        formatter: col=> new CellFormatter((cell: string) => cell && (cell.substr(0, 5) + "…" + cell.substring(cell.length - 5)))
    },
    {
        name: "DateTime",
        isApplicable: col=> col.token.filterType == FilterType.DateTime,
        formatter: col=> {
            const momentFormat = toMomentFormat(col.token.format);
            return new CellFormatter((cell: string) => cell == null || cell == "" ? "" : moment(cell).format(momentFormat))
        }
    },
    {
        name: "Number",
        isApplicable: col=> col.token.filterType == FilterType.Integer || col.token.filterType == FilterType.Decimal,
        formatter: col=> new CellFormatter((cell: number) => cell && cell.toString())
    },
    {
        name: "Number with Unit",
        isApplicable: col=> (col.token.filterType == FilterType.Integer || col.token.filterType == FilterType.Decimal) && !!col.token.unit,
        formatter: col=> new CellFormatter((cell: number) => cell && cell.toString() + " " + col.token.unit)
    },
    {
        name: "Bool",
        isApplicable: col=> col.token.filterType == FilterType.Boolean,
        formatter: col=> new CellFormatter((cell: boolean) => cell == null ? null : <input type="checkbox" disabled={true} checked={cell}/>)
    },
];




export interface EntityFormatRule {
    name: string;
    formatter: EntityFormatter;
    isApplicable: (row: ResultRow) => boolean;
}


export type EntityFormatter = (row: ResultRow) => React.ReactNode;

export const entityFormatRules: EntityFormatRule[] = [
    {
        name: "View",
        isApplicable: row=> true,
        formatter: row=> !isNavigable(row.entity.EntityType, null, true) ? null :
            <Link to={navigateRoute(row.entity) } title={row.entity.toStr} data-entity-link={liteKey(row.entity) }>
                {EntityControlMessage.View.niceToString() }
                </Link>
    },
];








