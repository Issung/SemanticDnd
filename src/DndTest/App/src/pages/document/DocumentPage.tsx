import { useEffect, useState } from 'react';
import { useParams } from '@tanstack/react-router'
import { useDocument } from '@/hooks/api/useDocument';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { CustomFields } from '@/components/CustomFields';

export default function DocumentPage() {
    const { id } = useParams({ strict: false }) // will be typed later with route param
    const { data, isPending, isError } = useDocument(id!);
    const [fileType, setFileType] = useState<null | 'text' | 'image' | 'pdf'>(null);
    const [fileContent, setFileContent] = useState<string | Blob | null>(null);
    const [fileError, setFileError] = useState<string | null>(null);

    useEffect(() => {
        const load = async () => {
            if (data?.document.fileAccessUrl) {
                setFileContent(null);
                setFileError(null);
                const res = await fetch(data.document.fileAccessUrl);

                if (!res.ok) {
                    setFileError('Failed to fetch file. Status: ' + res.status);
                }

                const ct = res.headers.get('Content-Type') ?? '';
                if (ct.startsWith('text/plain')) {
                    const text = await res.text();
                    setFileType('text');
                    setFileContent(text);
                }
                else if (ct.startsWith('image/') || ct.startsWith('application/pdf')) {
                    const blob = await res.blob();
                    setFileType(ct.startsWith('image/') ? 'image' : 'pdf');
                    setFileContent(blob);
                }
                else {
                    setFileError('Unsupported file type: ' + ct);
                }
            }
        }

        load();
    }, [data?.document.fileAccessUrl]);

    return <>
        <div>
            {isPending && <p>Loading...</p>}
            {isError && <p>Error loading document</p>}
            {data && (
                <div>
                    <h1>{data.document.name}</h1>
                    <CustomFields fields={data.document.customFields}/>
                    {data.document.text && <ReactMarkdown remarkPlugins={[remarkGfm]}>{data.document.text}</ReactMarkdown>}
                    {data.document.fileAccessUrl && (
                        <>
                            <a href={data.document.fileAccessUrl} target="_blank" rel="noopener noreferrer">
                                Download File
                            </a>
                            <div style={{ marginTop: '1em' }}>
                                <h3>File Contents:</h3>
                                {fileError && <p style={{ color: 'red' }}>Error: {fileError}</p>}
                                {fileContent === null && !fileError && <p>Loading file...</p>}
                                {fileContent !== null && (
                                    fileType === 'image' ? (
                                        <img src={URL.createObjectURL(fileContent as Blob)} />
                                    ) : fileType === 'pdf' ? (
                                        <embed src={URL.createObjectURL(fileContent as Blob)} />
                                    ) : fileType === 'text' ? (
                                    <pre style={{ padding: '1em', textWrap: 'wrap' }}>{fileContent as string}</pre>
                                    ) : <></>
                                )}
                            </div>
                        </>
                    )}
                </div>
            )}
        </div>
    </>
}
