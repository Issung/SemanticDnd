import ItemList, { ItemListDisplay } from "./components/ItemsList";
import { useItems } from "./hooks/api/useItems";

export const ContentPage = () => {
    console.log('ContentPage');

    const {data, isPending, isError} = useItems();

    return (
        isPending ? "Loading..." :
        isError ? "Error" :
        <ItemList hits={data.items.map(ItemListDisplay.fromSummary)} />
    );
}