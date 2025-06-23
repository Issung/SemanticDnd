import { useNavigate } from "@tanstack/react-router";
import BookmarkCollectionsList from "./components/BookmarkCollectionsList";
import { setHeader } from "./components/HeaderContext";

export const BookmarkCollectionsPage = () => {
    console.log('BookmarkCollectionsPage');

    setHeader({back: false, title: 'Bookmark Collections'});

    const navigate = useNavigate();

    return (
        <>
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