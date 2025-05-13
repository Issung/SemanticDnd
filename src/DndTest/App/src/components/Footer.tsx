import FolderIcon from '@mui/icons-material/Folder';
import HomeIcon from '@mui/icons-material/Home';
import StarIcon from '@mui/icons-material/Star';
import ButtonBase from "@mui/material/ButtonBase";
import { useNavigate } from "@tanstack/react-router";

export const Footer = () => {
    const navigate = useNavigate();

    return <div id="footer">
        <ButtonBase onClick={() => navigate({to: '/'})}>
            <HomeIcon htmlColor='white'/>
        </ButtonBase>
        <ButtonBase onClick={() => navigate({to: '/'})}>
            <FolderIcon htmlColor='white'/>
        </ButtonBase>
        <ButtonBase onClick={() => navigate({to: '/'})}>
            <StarIcon htmlColor='white'/>
        </ButtonBase>
    </div>
};