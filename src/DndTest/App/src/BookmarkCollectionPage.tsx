import { useParams } from "@tanstack/react-router";
import { useBookmarkCollection } from "./hooks/api/useBookmarkCollection";
import { useBookmarkCollectionItems } from "./hooks/api/useBookmarkCollectionItems";
import ItemList, { ItemListDisplay } from "./components/ItemsList";

export function BookmarkCollectionPage() {
    const { id } = useParams({ strict: false }) // will be typed later with route param
    const {data: collectionData} = useBookmarkCollection(id!);
    const {data: itemsData} = useBookmarkCollectionItems(id!);

    const items = itemsData?.items.map(ItemListDisplay.fromSummary);

    return (
        <>
            {!collectionData
                ? <h1>Bookmark Collection</h1>
                : <>
                    <h1>{collectionData.bookmarkCollection.name}</h1>
                    <p>{collectionData.bookmarkCollection.description}</p>
                    {!items
                        ? "Items loading..."
                        : <ItemList hits={items}/>
                    }
                </>
            }
        </>
    );
}