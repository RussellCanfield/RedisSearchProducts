import { useContext, useState } from "react";
import "./App.css";
import { ProductContext } from "./context/ProductContext";
import FilterList from "./features/Filters/List/FilterList";
import ProductList from "./features/Products/List/ProductList";
import SearchBar from "./features/Search/SearchBar";

function App() {
	return (
		<>
			<SearchBar></SearchBar>
			<div className="main-page">
				<FilterList></FilterList>
				<ProductList></ProductList>
			</div>
		</>
	);
}

export default App;
