import FolderIcon from '@mui/icons-material/Folder';
import HomeIcon from '@mui/icons-material/Home';
import SearchIcon from '@mui/icons-material/Search';
import ButtonBase from "@mui/material/ButtonBase";
import BookmarksIcon from '@mui/icons-material/Bookmarks';
import { useNavigate } from "@tanstack/react-router";

export const Footer = () => {
    const navigate = useNavigate();

    return <div id="footer">
        <ButtonBase onClick={() => navigate({to: '/content'})}>
            <FolderIcon htmlColor='white'/>
        </ButtonBase>
        <ButtonBase onClick={() => navigate({to: '/search'})}>
            <SearchIcon htmlColor='white'/>
        </ButtonBase>
        <ButtonBase onClick={() => navigate({to: '/bookmarks'})}>
            <BookmarksIcon htmlColor='white'/>
        </ButtonBase>
    </div>
};