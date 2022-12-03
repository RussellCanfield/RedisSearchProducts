export interface Filter {
	name: string;
	values: FilterValue[];
}

export interface FilterValue {
	name: string;
	count: number;
}
