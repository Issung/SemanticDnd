import {
    Box,
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    Paper,
    Stack,
    Typography,
    styled,
} from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import type { Crop } from "react-image-crop";
import ReactCrop from "react-image-crop";
import 'react-image-crop/dist/ReactCrop.css';

const DragDropArea = styled(Paper)(
    ({ theme, isdragover }: { theme?: any; isdragover: boolean }) => ({
        padding: theme.spacing(3),
        textAlign: "center",
        border: `2px dashed ${isdragover ? theme.palette.primary.main : theme.palette.grey[400]
            }`,
        backgroundColor: isdragover
            ? theme.palette.action.hover
            : theme.palette.background.default,
        cursor: "pointer",
        minHeight: 200,
        display: "flex",
        flexDirection: "column",
alignItems: "center",
        justifyContent: "center",
        gap: theme.spacing(2),
    })
);

const FilePreview = ({ file }: { file: File }) => {
    const [previewUrl, setPreviewUrl] = useState<string | null>(null);

    useEffect(() => {
        if (file.type.startsWith("image/"))
        {
            const reader = new FileReader();
            reader.onload = () => setPreviewUrl(reader.result as string);
            reader.readAsDataURL(file);
        } else
        {
            setPreviewUrl(null);
        }

        return () => {
            if (previewUrl) URL.revokeObjectURL(previewUrl);
        };
    }, [file]);

    return (
        <>
            {previewUrl ? (
                <Box
                    component="img"
                    src={previewUrl}
                    alt={file.name}
                    sx={{ maxWidth: 180, maxHeight: 180, borderRadius: 2, boxShadow: 1 }}
                />
            ) : (
                <Typography variant="body2">📄 {file.name}</Typography>
            )}
        </>
    );
};

interface FileUploadProps {
    onFileChange: (file: File | null) => void;
}

function FileUpload(props: FileUploadProps) {
    const [file, setFile] = useState<File | null>(null);
    const [isDragOver, setIsDragOver] = useState(false);
    const [crop, setCrop] = useState<Crop>();
    const completedCrop = crop;
    // const [completedCrop, setCompletedCrop] = useState<PixelCrop>();
    const [sourceImageUrl, setSourceImageUrl] = useState<string | null>(null);
    const [isCropModalOpen, setIsCropModalOpen] = useState(false);
    const cropImgRef = useRef<HTMLImageElement>(null);

    const fileInputRef = useRef<HTMLInputElement>(null);
    const cameraInputRef = useRef<HTMLInputElement>(null);

    const handleFileSelect = (files: FileList | null) => {
        if (files && files.length > 0) {
            if (files[0].type.startsWith('image/')) {
                const reader = new FileReader();
                reader.onload = () => {
                    setSourceImageUrl(reader.result as string);
                    setIsCropModalOpen(true);
                };
                reader.readAsDataURL(files[0]);
            } else {
                setFile(files[0]);
                props.onFileChange(files[0]);
            }
        }
    };

    const onImageLoad = (e: React.SyntheticEvent<HTMLImageElement>) => {
        const { width, height } = e.currentTarget;
        setCrop({
            unit: 'px',
            width: width,
            height: height,
            x: 0,
            y: 0,
        })
    }

    const upload = (e: React.MouseEvent) => {
        e.stopPropagation();
        fileInputRef.current?.click();
    };

    const openCamera = (e: React.MouseEvent) => {
        e.stopPropagation();
        cameraInputRef.current?.click();
    };

    const remove = (e: React.MouseEvent) => {
        e.stopPropagation();
        setFile(null);
        props.onFileChange(null);
    };

    const handleCropDone = () => {
        if (completedCrop && cropImgRef.current) {
            const canvas = document.createElement('canvas');
            const scaleX = cropImgRef.current.naturalWidth / cropImgRef.current.width;
            const scaleY = cropImgRef.current.naturalHeight / cropImgRef.current.height;
            canvas.width = completedCrop.width;
            canvas.height = completedCrop.height;
            const ctx = canvas.getContext('2d');

            if (ctx) {
                ctx.drawImage(
                    cropImgRef.current,
                    completedCrop.x * scaleX,
                    completedCrop.y * scaleY,
                    completedCrop.width * scaleX,
                    completedCrop.height * scaleY,
                    0,
                    0,
                    completedCrop.width,
                    completedCrop.height
                );
                canvas.toBlob((blob) => {
                    if (blob) {
                        const croppedFile = new File([blob], "cropped_image.png", { type: "image/png" });
                        setFile(croppedFile);
                        props.onFileChange(croppedFile);
                        setIsCropModalOpen(false);
                    }
                }, 'image/png');
            }
        }
    };

    return (
        <Box>
            <DragDropArea
                elevation={0}
                isdragover={isDragOver}
                onDragOver={(e) => {
                    e.preventDefault();
                    setIsDragOver(true);
                }}
                onDragLeave={() => setIsDragOver(false)}
                onDrop={(e) => {
                    e.preventDefault();
                    setIsDragOver(false);
                    handleFileSelect(e.dataTransfer.files);
                }}
                onClick={upload}
            >
                {file ? (
                    <>
                        <FilePreview file={file} />
                        <Stack direction="row" gap={1}>
                            <Button variant="outlined" onClick={upload}>
                                Swap
                            </Button>
                            <Button variant="outlined" onClick={openCamera}>
                                Camera
                            </Button>
                            <Button variant="outlined" onClick={remove}>
                                Remove
                            </Button>
                        </Stack>
                    </>
                ) : (
                    <>
                        <Typography variant="body1">
                            Drag & drop a file here or click to upload
                        </Typography>
                        <Stack direction="row" gap={1}>
                            <Button variant="outlined" onClick={upload}>
                                Upload
                            </Button>
                            <Button variant="outlined" onClick={openCamera}>
                                Camera
                            </Button>
                        </Stack>
                    </>
                )}
            </DragDropArea>

            <input
                type="file"
                ref={fileInputRef}
                hidden
                onChange={(e) => handleFileSelect(e.target.files)}
            />

            <input
                type="file"
                ref={cameraInputRef}
                hidden
                accept="image/*"
                capture="environment"
                onChange={(e) => handleFileSelect(e.target.files)}
            />

            <Dialog open={isCropModalOpen} onClose={() => setIsCropModalOpen(false)} maxWidth="lg">
                <DialogContent>
                    {sourceImageUrl && (
                        <ReactCrop
                            crop={crop}

                            onChange={c =>  setCrop(c)} // Fires while interacting (dragging).
                            // onComplete={c => setCompletedCrop(c)}    // Fires when finishing interaction (drop).
                        >
                            <img ref={cropImgRef} src={sourceImageUrl} onLoad={onImageLoad} />
                        </ReactCrop>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setIsCropModalOpen(false)}>Cancel</Button>
                    <Button onClick={handleCropDone} variant="contained">Done</Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
}


export default FileUpload;
