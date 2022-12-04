import { useCallback, useContext, useState } from "react";
import baseUrl from "../../config";
import { ProductContext } from "../../context/ProductContext";
import styles from "./SearchBar.module.css";

let timeout = 0;

const SearchBar = () => {
	const { searchResults, setSearchText } = useContext(ProductContext);
	const [suggestions, setSuggestions] = useState<string[]>([]);

	const executeSearch = async (value: string) => {
		const response = await fetch(
			`${baseUrl}/product/suggestion?searchTerm=${value}`
		);

		const result = (await response.json()) as string[];
		setSuggestions(result);
	};

	const searchShirts = useCallback((searchTerm: string) => {
		if (!timeout) {
			timeout = setTimeout(() => executeSearch(searchTerm), 250);
			return;
		}

		clearTimeout(timeout);
		timeout = setTimeout(() => executeSearch(searchTerm), 250);
	}, []);

	const selectSuggestion = (suggestion: string) => {
		setSearchText(suggestion);
	};

	return (
		<div className={styles["search-container"]}>
			<div className={styles["search-results"]}>
				<input
					type="text"
					placeholder="Search"
					className={styles["search-bar"]}
					onChange={(e) => searchShirts(e.target.value)}
				></input>
				{suggestions.length > 0 && (
					<div className={styles["search-suggestions"]}>
						{suggestions.map((s) => {
							return (
								<div
									className={styles["search-suggestion"]}
									onClick={() => selectSuggestion(s)}
								>
									{s}
								</div>
							);
						})}
					</div>
				)}
			</div>
			<div className={styles["product-count"]}>
				{searchResults.total} - Shirts
			</div>
		</div>
	);
};

export default SearchBar;
