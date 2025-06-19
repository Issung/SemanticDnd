import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { Button } from '@mui/material';
import { useRouter } from '@tanstack/react-router';


export default function Header() {
    const { history } = useRouter()
    
    return <div id="header">
        <Button size="small" variant="contained" color="primary" onClick={() => history.back()} startIcon={<ArrowBackIcon />}>Back</Button>
        <h1 style={{ margin: '0 0 0 10px', height: '100%' }}>
            {/* Header Here */}
        </h1>
    </div>
}
