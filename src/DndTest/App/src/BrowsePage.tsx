import { setHeader } from "./components/HeaderContext";
import ItemList, { ItemListDisplayAdapter } from "./components/ItemsList";
import { useBrowse } from "./hooks/api/useBrowse";
import { browseRoute } from "./main";

export default function BrowsePage() {
    console.log('BrowsePage');

    setHeader({ back: false, title: 'Content' });

    const { folderId } = browseRoute.useParams();

    const { data, isPending, isError } = useBrowse(folderId);

    return (
        isPending ? "Loading..." :
            isError ? "Error" :
                <ItemList hits={data.items.map(ItemListDisplayAdapter.fromSummary)} />
    );
}