import { useNavigate } from "@tanstack/react-router";
import BookmarkCollectionsList from "./components/BookmarkCollectionsList";

export const BookmarkCollectionsPage = () => {
    console.log('BookmarkCollectionsPage');

    const navigate = useNavigate();

    return (
        <>
            <h1>Bookmarks</h1>
            <BookmarkCollectionsList
                onSelect={(id) => navigate({
                    to: '/bookmarkCollection/$id',
                    params: { id: id}
                })}
            >
            </BookmarkCollectionsList>
        </>
    );
}