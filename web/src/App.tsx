import { useContext, useState } from "react";
import "./App.css";
import { ProductContext } from "./context/ProductContext";
import FilterList from "./features/Filters/List/FilterList";
import ProductList from "./features/Products/List/ProductList";

function App() {
	const { setFilter } = useContext(ProductContext);

	return (
		<div className="main-page">
			<FilterList></FilterList>
			<ProductList></ProductList>
		</div>
	);
}

export default App;
