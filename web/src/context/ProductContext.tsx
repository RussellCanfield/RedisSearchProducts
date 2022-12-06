import {
	createContext,
	ReactNode,
	useCallback,
	useEffect,
	useState,
} from "react";
import baseUrl from "../config";
import { Filter } from "../types/Filter";
import { Product } from "../types/Product";
import {
	SearchRequest,
	SearchRequestFilter,
	SearchRequestRange,
	SearchResponse,
} from "../types/Search";

interface IProductContext {
	searchResults: SearchResponse;
	filters: string[];
	pageNumber: number;
	pageSize: number;
	setPage: (page: number) => void;
	setFilter: (filterName: string, filterValue: string) => void;
	setRange: (min: string, max: string) => void;
	setSearchText: (searchTerm: string) => void;
}

const productAppContext: IProductContext = {
	searchResults: {
		total: 0,
		count: 0,
		products: [],
	},
	filters: [],
	pageNumber: 1,
	pageSize: 25,
	setPage: (_page) => {},
	setFilter: (_filterName, _filterValue) => {},
	setRange: (_min, _max) => {},
	setSearchText: (_searchTerm) => {},
};

export const ProductContext = createContext<IProductContext>(productAppContext);

interface Props {
	children: ReactNode;
}

export const ProductContextProvider = ({ children }: Props) => {
	const [searchFilters, setSearchFilters] = useState<SearchRequestFilter[]>(
		[]
	);
	const [filters, setFilters] = useState<string[]>([]);
	const [searchResults, setSearchResults] = useState<SearchResponse>({
		total: 0,
		count: 0,
		products: [],
	});
	const [searchTerm, setSearchTerm] = useState<string | undefined>(undefined);
	const [pageSize, setPageSize] = useState<number>(25);
	const [pageNumber, setPageNumber] = useState<number>(1);
	const [searchRange, setSearchRange] = useState<
		SearchRequestRange | undefined
	>();

	const setFilter = useCallback((filterName: string, filterValue: string) => {
		setSearchFilters((filters) => {
			const filterIndex = filters.findIndex((f) => f.name === filterName);

			if (filterIndex === -1) {
				filters.push({
					name: filterName,
					values: [filterValue],
				});
			} else {
				if (!filters[filterIndex].values.includes(filterValue)) {
					filters[filterIndex] = {
						name: filterName,
						values: [...filters[filterIndex].values, filterValue],
					};
				} else {
					const remainingFilters = filters[filterIndex].values.filter(
						(v) => v !== filterValue
					);

					if (remainingFilters.length === 0) {
						filters = filters.filter((f) => f.name !== filterName);
					} else {
						filters[filterIndex] = {
							name: filterName,
							values: [...remainingFilters],
						};
					}
				}
			}

			return [...filters];
		});
		setPageNumber(1);
	}, []);

	const search = useCallback(
		async (searchRequest: SearchRequest) => {
			const response = await fetch(`${baseUrl}/product/search`, {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
				},
				body: JSON.stringify(searchRequest),
			});

			const results = (await response.json()) as SearchResponse;
			setSearchResults(results);
		},
		[setSearchResults]
	);

	const getFilters = useCallback(async () => {
		const response = await fetch(`${baseUrl}/product/filters`, {
			method: "GET",
		});

		const results = (await response.json()) as string[];
		setFilters(results);
	}, [setFilters]);

	const setPage = useCallback((page: number) => {
		setPageNumber(page);
	}, []);

	const setSearchText = useCallback((searchTerm: string) => {
		setSearchTerm(searchTerm);
		setPageNumber(1);
	}, []);

	const setRange = useCallback((min: string, max: string) => {
		setSearchRange({
			name: "Price",
			min,
			max,
		});
		setPageNumber(1);
	}, []);

	useEffect(() => {
		search({
			text: searchTerm,
			pageSize,
			pageNumber,
			filters: searchFilters,
			range: searchRange,
		});

		getFilters();
	}, [searchTerm, searchFilters, searchRange, pageNumber, pageSize]);

	return (
		<ProductContext.Provider
			value={{
				searchResults,
				filters,
				pageNumber,
				pageSize,
				setPage,
				setFilter,
				setRange,
				setSearchText,
			}}
		>
			{children}
		</ProductContext.Provider>
	);
};
