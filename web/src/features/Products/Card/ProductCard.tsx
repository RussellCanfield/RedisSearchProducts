import { Product, ProductColor } from "../../../types/Product";
import styles from "./ProductCard.module.css";

interface Props {
	product: Product;
}

const numberFormatter = new Intl.NumberFormat(undefined, {
	style: "currency",
	currency: "USD",
});

const ProductCard = ({ product }: Props) => {
	const getShirtImg = (color: ProductColor) => {
		if (color === ProductColor.Black) {
			return "./assets/BlackShirt.jpeg";
		} else if (color === ProductColor.Blue) {
			return "./assets/BlueShirt.jpeg";
		} else if (color === ProductColor.Grey) {
			return "./assets/GreyShirt.jpeg";
		} else if (color === ProductColor.Red) {
			return "./assets/RedShirt.jpeg";
		} else if (color === ProductColor.White) {
			return "./assets/WhiteShirt.jpeg";
		}
	};

	const formatCurrency = (price: number) => {
		return numberFormatter.format(price);
	};

	return (
		<article className={styles["product-card"]}>
			<div>
				<img
					src={getShirtImg(product.color)}
					className={styles["product-card-image"]}
				></img>
			</div>
			<div>
				<div className={styles["product-card-title"]}>
					{product.name}
				</div>
				<div>{formatCurrency(product.price)}</div>
			</div>
		</article>
	);
};

export default ProductCard;
