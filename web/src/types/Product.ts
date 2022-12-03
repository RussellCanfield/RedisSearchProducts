export interface Product {
	id: string;
	name: string;
	price: number;
	color: ProductColor;
	size: string;
}

export enum ProductColor {
	White = "White",
	Black = "Black",
	Red = "Red",
	Blue = "Blue",
	Grey = "Grey",
}
