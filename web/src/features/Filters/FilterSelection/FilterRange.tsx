import { createRef, useContext, useRef } from "react";
import { ProductContext } from "../../../context/ProductContext";
import styles from "./FilterSelection.module.css";

interface Props {
	filterName: string;
}

const FilterRange = ({ filterName }: Props) => {
	const { setRange } = useContext(ProductContext);
	const minRef = useRef<HTMLInputElement>(null);
	const maxRef = useRef<HTMLInputElement>(null);

	const applyRange = () => {
		if (!minRef.current || !maxRef.current) return;

		setRange(minRef.current.value, maxRef.current.value);
	};

	return (
		<div>
			<>
				<div className={styles["filter-selection-title"]}>
					{filterName}
				</div>
				<div className={styles["filter-ranges"]}>
					<input
						type="text"
						placeholder="10"
						ref={minRef}
						onBlur={applyRange}
					/>
					<input
						type="text"
						ref={maxRef}
						placeholder="200"
						onBlur={applyRange}
					/>
				</div>
			</>
		</div>
	);
};

export default FilterRange;
