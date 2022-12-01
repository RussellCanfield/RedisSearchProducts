import { useContext, useState } from "react";
import reactLogo from "./assets/react.svg";
import "./App.css";
import { ProductContext } from "./context/ProductContext";

function App() {
	const { products, setFilter } = useContext(ProductContext);

	const addFilter = () => {
		setFilter("color", "Blue");
	};

	return (
		<div>
			<button type="button" onClick={() => addFilter()}>
				Test
			</button>
			<section>
				{products &&
					products.map((product) => {
						return JSON.stringify(product);
					})}
			</section>
		</div>
	);
}

export default App;
