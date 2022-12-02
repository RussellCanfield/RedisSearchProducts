import { useContext, useState } from "react";
import "./App.css";
import { ProductContext } from "./context/ProductContext";
import ProductList from "./components/ProductList";

function App() {
	const { setFilter } = useContext(ProductContext);

	return (
		<div>
			<ProductList></ProductList>
		</div>
	);
}

export default App;
