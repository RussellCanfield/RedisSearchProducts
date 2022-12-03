import {
	createContext,
	ReactNode,
	useCallback,
	useEffect,
	useState,
} from "react";
import { Filter } from "../types/Filter";
import { Product } from "../types/Product";
import {
	SearchRequest,
	SearchRequestFilter,
	SearchResponse,
} from "../types/Search";

interface IProductContext {
	searchResults: SearchResponse;
	filters: string[];
	pageNumber: number;
	pageSize: number;
	setFilter: (filterName: string, filterValue: string) => void;
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
	setFilter: (_filterName, _filterValue) => {},
};

export const ProductContext = createContext<IProductContext>(productAppContext);

interface Props {
	children: ReactNode;
}

const baseUrl = "https://localhost:7009";

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
	const [pageSize, setPageSize] = useState<number>(25);
	const [pageNumber, setPageNumber] = useState<number>(1);

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

			const updatedFilters = [...filters];

			search({
				pageSize,
				pageNumber,
				filters: updatedFilters,
			});

			return updatedFilters;
		});
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
		search({
			pageSize,
			pageNumber: page,
			filters: searchFilters,
		});
	}, []);

	useEffect(() => {
		search({
			pageSize,
			pageNumber,
			filters: searchFilters,
		});

		getFilters();
	}, []);

	return (
		<ProductContext.Provider
			value={{
				searchResults,
				filters,
				pageNumber,
				pageSize,
				setFilter,
			}}
		>
			{children}
		</ProductContext.Provider>
	);
};
