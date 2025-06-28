import { setHeader } from "./components/HeaderContext";
import ItemList, { ItemListDisplayAdapter } from "./components/ItemsList";
import { useBrowse } from "./hooks/api/useBrowse";
import { browseRoute } from "./main";

export default function BrowsePage() {
    console.log('BrowsePage');
    
    const { folderId } = browseRoute.useParams();
    const { data, isPending, isError } = useBrowse(folderId);

    setHeader({
        back: Boolean(folderId),    // If within a folder, then display back button.
        title: 'Content',    // TODO: Browse response should probably include folder name, and maybe parent folder name for back button? To display in header.
    });

    return (
        isPending ? "Loading..." :
            isError ? "Error" :
                <ItemList hits={data.items.map(ItemListDisplayAdapter.fromSummary)} />
    );
}