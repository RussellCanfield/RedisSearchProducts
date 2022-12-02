import { useContext } from "react";
import { ProductContext } from "../../../context/ProductContext";
import ProductCard from "../Card/ProductCard";

import styles from "./ProductList.module.css";

const ProductList = () => {
	const { products } = useContext(ProductContext);

	if (!products || products.length === 0) return <></>;

	return (
		<section className={styles["product-list"]}>
			{products.map((product) => {
				return (
					<ProductCard
						product={product}
						key={product.id}
					></ProductCard>
				);
			})}
		</section>
	);
};

export default ProductList;
