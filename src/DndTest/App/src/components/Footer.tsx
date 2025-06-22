import BookmarksIcon from '@mui/icons-material/Bookmarks';
import FolderIcon from '@mui/icons-material/Folder';
import SearchIcon from '@mui/icons-material/Search';
import ButtonBase from "@mui/material/ButtonBase";
import { useNavigate } from "@tanstack/react-router";

export const Footer = () => {
    const navigate = useNavigate();

    return <div id="footer">
        <ButtonBase onClick={() => navigate({to: '/content'})}>
            <FolderIcon htmlColor='white'/>
        </ButtonBase>
        <ButtonBase onClick={() => navigate({to: '/search', search: { query: "" }})}>
            <SearchIcon htmlColor='white'/>
        </ButtonBase>
        <ButtonBase onClick={() => navigate({to: '/bookmarkCollections'})}>
            <BookmarksIcon htmlColor='white'/>
        </ButtonBase>
    </div>
};