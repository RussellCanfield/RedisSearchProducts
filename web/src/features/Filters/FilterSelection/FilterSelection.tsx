import { useEffect, useState } from "react";
import { Filter } from "../../../types/Filter";
import styles from "./Filter.module.css";

interface Props {
	filterName: string;
}

const baseUrl = "https://localhost:7009";

const FilterSelection = ({ filterName }: Props) => {
	const [filter, setFilter] = useState<Filter | undefined>();

	useEffect(() => {
		async function loadFilter() {
			const response = await fetch(
				`${baseUrl}/product/filters/${filterName}`,
				{
					method: "GET",
				}
			);

			const result = (await response.json()) as Filter;
			setFilter(result);
		}

		if (!filter) {
			loadFilter();
		}
	});

	return (
		<div>
			<>
				<div>{filterName}</div>
				<ul>
					{filter?.values.map((value) => {
						return (
							<li key={value.name}>
								<span>{value.name}</span>
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
