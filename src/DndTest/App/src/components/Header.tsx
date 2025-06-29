import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { Button } from '@mui/material';
import { useRouter } from '@tanstack/react-router';
import { useHeaderConfig } from './HeaderContext';


export default function Header() {
    const { history } = useRouter()
    const headerConfig = useHeaderConfig();
    
    return <div id="Header">
        {/* Left */}
        <div>
            {headerConfig.back && <Button size="small" variant="contained" color="primary" onClick={() => history.back()} startIcon={<ArrowBackIcon />}>Back</Button>}
            {headerConfig.title &&
                <h1 style={{ margin: '0 0 0 10px', height: '100%' }}>
                    {headerConfig.title}
                </h1>
            }
        </div>
        {/* Right */}
        <div id="Header__Adornment">
            {headerConfig.adornment}
        </div>
    </div>
}
