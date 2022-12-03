import { useContext, useEffect, useState } from "react";
import { ProductContext } from "../../../context/ProductContext";
import { Filter } from "../../../types/Filter";
import styles from "./FilterSelection.module.css";

interface Props {
	filterName: string;
}

const baseUrl = "https://localhost:7009";

const FilterSelection = ({ filterName }: Props) => {
	const { setFilter } = useContext(ProductContext);
	const [filters, setAvailableFilters] = useState<Filter | undefined>();

	useEffect(() => {
		async function loadFilter() {
			const response = await fetch(
				`${baseUrl}/product/filters/${filterName}`,
				{
					method: "GET",
				}
			);

			const result = (await response.json()) as Filter;
			setAvailableFilters(result);
		}

		if (!filters) {
			loadFilter();
		}
	});

	const applyFilter = (filterValue: string) => {
		setFilter(filterName, filterValue);
	};

	return (
		<div>
			<>
				<div className={styles["filter-selection-title"]}>
					{filterName}
				</div>
				<ul className={styles["filter-selection-list"]}>
					{filters?.values.map((value) => {
						return (
							<li
								key={value.name}
								className={styles["filter-selection-list-item"]}
							>
								<span>
									<input
										type="checkbox"
										id={value.name}
										name={value.name}
										value={""}
										onClick={() => applyFilter(value.name)}
									/>
									<label htmlFor={value.name}>
										{value.name}
									</label>
								</span>
								<span>{value.count}</span>
							</li>
						);
					})}
				</ul>
			</>
		</div>
	);
};

export default FilterSelection;
