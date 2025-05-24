import { useDocuments } from "./hooks/api/useDocuments"

export const Content = () => {
    const {data, isPending, isError} = useDocuments();

    return (
        isPending ? "Loading..." :
        isError ? "Error" : 
        data.documents.map((document) => (
            <div key={document.id}>
                <h2>{document.name}</h2>
                <p>{document.category}</p>
            </div>
        ))
    );
}