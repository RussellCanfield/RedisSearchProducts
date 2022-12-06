import { useContext } from "react";
import { ProductContext } from "../../../context/ProductContext";
import FilterRange from "../FilterSelection/FilterRange";
import FilterSelection from "../FilterSelection/FilterSelection";
import styles from "./FilterList.module.css";

const FilterList = () => {
	const { filters } = useContext(ProductContext);

	return (
		<section className={styles["filter-list"]}>
			{filters.map((filter) => {
				return (
					<FilterSelection
						key={filter}
						filterName={filter}
					></FilterSelection>
				);
			})}
			<FilterRange filterName="Price"></FilterRange>
		</section>
	);
};

export default FilterList;
