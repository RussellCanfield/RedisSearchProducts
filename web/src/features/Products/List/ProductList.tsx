import { useContext } from "react";
import { ProductContext } from "../../../context/ProductContext";
import ProductCard from "../Card/ProductCard";

import styles from "./ProductList.module.css";

const ProductList = () => {
	const { searchResults, pageSize, pageNumber } = useContext(ProductContext);

	const maxPages = Math.ceil(searchResults.total / pageSize);

	let numOfPages: number[] = Array.from(
		{ length: maxPages < pageNumber + 10 ? maxPages : pageNumber + 10 },
		(_, i) => i + 1
	);

	if (searchResults.total === 0) return <></>;

	return (
		<div className={styles["product-container"]}>
			<section className={styles["product-pages"]}>
				{numOfPages.map((page) => {
					return <span>{page}</span>;
				})}
			</section>
			<section className={styles["product-list"]}>
				{searchResults.products.map((product) => {
					return (
						<ProductCard
							product={product}
							key={product.id}
						></ProductCard>
					);
				})}
			</section>
		</div>
	);
};

export default ProductList;
