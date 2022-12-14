import { Product } from "./Product";

export interface SearchRequest {
	text?: string;
	pageSize: number;
	pageNumber: number;
	filters: SearchRequestFilter[];
	range?: SearchRequestRange;
	sort?: SearchRequestSort;
}

export interface SearchRequestFilter {
	name: string;
	values: string[];
}

export interface SearchRequestRange {
	name: string;
	min: string;
	max: string;
}

export interface SearchRequestSort {
	name: string;
	direction: SearchSortDirection;
}

export enum SearchSortDirection {
	Ascending,
	Descending,
}

export interface SearchResponse {
	count: number;
	total: number;
	products: Product[];
}
