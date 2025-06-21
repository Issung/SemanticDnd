import { useItems } from "./hooks/api/useItems"

export const Content = () => {
    const {data, isPending, isError} = useItems();

    return (
        isPending ? "Loading..." :
        isError ? "Error" : 
        data.items.map((item) => (
            <div key={item.id}>
                <h2>{item.name}</h2>
                <p>{item.category}</p>
            </div>
        ))
    );
}