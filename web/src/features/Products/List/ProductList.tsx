import { useContext } from "react";
import { ProductContext } from "../../../context/ProductContext";
import ProductCard from "../Card/ProductCard";

import styles from "./ProductList.module.css";

const ProductList = () => {
	const { searchResults, pageSize, pageNumber, setPage } =
		useContext(ProductContext);

	const maxPages = Math.ceil(searchResults.total / pageSize);
	const numberOfDisplayPages =
		maxPages - pageNumber + 5 < 10 ? maxPages - pageNumber + 5 : 10;

	let numOfPages: number[] = Array.from(
		{ length: numberOfDisplayPages },
		(_, i) => {
			const currentPageNumber = Math.max(pageNumber - 5, 0) + i + 1;
			return currentPageNumber;
		}
	);

	if (searchResults.total === 0) return <></>;

	const selectPage = (page: number) => {
		setPage(page);
	};

	return (
		<div className={styles["product-container"]}>
			<section className={styles["product-pages"]}>
				<div onClick={() => selectPage(1)}>&#60;</div>
				{numOfPages.map((page) => {
					return (
						<div
							key={page}
							className={
								page === pageNumber ? styles["active-page"] : ""
							}
							onClick={() => selectPage(page)}
						>
							{page}
						</div>
					);
				})}
				<div onClick={() => selectPage(maxPages)}>&#62;</div>
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
