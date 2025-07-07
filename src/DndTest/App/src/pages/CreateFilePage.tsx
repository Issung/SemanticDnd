import FileUpload from '@/components/FileUpload';
import { setHeader } from '@/components/HeaderContext';
import { useItemPut } from '@/hooks/api/useItemPut';
import { createFileRoute } from '@/main';
import SaveIcon from '@mui/icons-material/Save';
import { IconButton, Stack, TextField } from '@mui/material';
import { useNavigate } from '@tanstack/react-router';
import { useState } from 'react';

export default function CreateFilePage() {
    const { parentId } = createFileRoute.useSearch();
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [file, setFile] = useState<File | null>(null);
    const { mutate, isPending } = useItemPut();
    const navigate = useNavigate();

    const valid = name.trim().length > 0 && file;
    
    function handleSave() {
        if (!valid) {
            return;
        }
        
        mutate({
            parentId,
            name,
            description,
            file,
        }, {
            onSuccess: (id) => {
                navigate({  // TODO: Add to `Navigations` class.
                    to: '/item/$id',
                    params: { id },
                    replace: true,
                });
            },
        });
    };
    
    setHeader({
        title: 'Create File',
        back: true,
        adornment:
            <IconButton onClick={handleSave} disabled={!valid} loading={isPending}>
                <SaveIcon />
            </IconButton>
    }, [valid, isPending]);
    
    console.log('CreateFilePage', { parentId, name, description });
    return <>
        <div>
            <Stack gap={2}>
                {/* TODO: Location select dialog (pre-fill with the location the user started creation in). */}

                <TextField label="Name" value={name} onChange={(e) => setName(e.target.value)} />

                <TextField label="Description" value={description} onChange={(e) => setDescription(e.target.value)} multiline />

                {/* TODO: Custom fields configuration & entry. API, models & hooks all made, just needs display code. */}

                <FileUpload onFileChange={setFile}/>
            </Stack>

            {/* <CustomFields fields={data.item.customFields} /> */}
        </div>
    </>
}
