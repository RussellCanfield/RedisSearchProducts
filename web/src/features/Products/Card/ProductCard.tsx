import { Product } from "../../../types/Product";
import styles from "./ProductCard.module.css";

interface Props {
	product: Product;
}

const ProductCard = ({ product }: Props) => {
	return (
		<article className={styles["product-card"]}>
			<div>{product.name}</div>
			<div>
				<img src={"./assets/GreyShirt.jpeg"}></img>
			</div>
			<div>{product.price}</div>
		</article>
	);
};

export default ProductCard;
