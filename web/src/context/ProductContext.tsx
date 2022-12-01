import {
	createContext,
	ReactNode,
	useCallback,
	useEffect,
	useState,
} from "react";
import { Product } from "../types/Product";
import {
	SearchRequest,
	SearchRequestFilter,
	SearchResponse,
} from "../types/Search";

interface IProductContext {
	products: Product[];
	setFilter: (filterName: string, filterValue: string) => void;
}

const productAppContext: IProductContext = {
	products: [],
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
	const [products, setProducts] = useState<Product[]>([]);
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
			} else if (!filters[filterIndex].values.includes(filterValue)) {
				filters[filterIndex] = {
					name: filterName,
					values: [...filters[filterIndex].values, filterValue],
				};
			}

			return [...filters];
		});
	}, []);

	const search = useCallback(async (searchRequest: SearchRequest) => {
		const response = await fetch(`${baseUrl}/product/search`, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
			},
			body: JSON.stringify(searchRequest),
		});

		const results = (await response.json()) as SearchResponse;
		setProducts(results.products);
	}, []);

	useEffect(() => {
		search({
			pageSize,
			pageNumber,
			filters: searchFilters,
		});
	}, [searchFilters]);

	return (
		<ProductContext.Provider
			value={{
				products,
				setFilter,
			}}
		>
			{children}
		</ProductContext.Provider>
	);
};
