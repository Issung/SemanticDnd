import type { ItemSummary, SearchHit } from "@/hooks/api/responses";
import { Divider, List, ListItem, ListItemText } from "@mui/material";
import { useNavigate } from "@tanstack/react-router";
import React from "react";

export class ItemListDisplay {
    constructor(
        public id: number,
        public name: string,
        public previewFields: Array<string>,
        public pageNumber: number | undefined
    ) {}

    static fromSummary(item: ItemSummary): ItemListDisplay {
        return new ItemListDisplay(
            item.id,
            item.name,
            item.previewFields,
            undefined // No page number in ItemSummary
        );
    }

    static fromSearchHit(hit: SearchHit): ItemListDisplay {
        return new ItemListDisplay(
            hit.item.id,
            hit.item.name,
            hit.item.previewFields,
            hit.pageNumber
        );
    }
}


const ItemList = ({ hits: items }: { hits: Array<ItemListDisplay> }) => {
    const navigate = useNavigate();

    return (
        <>
            <List>
                {items.map((item, index) => {
                    const key = `${item.id}${item.pageNumber ? `-${item.pageNumber}` : ''}`;

                    return <React.Fragment key={key}>
                        <ListItem
                            onClick={() => navigate({
                                to: '/item/$id',
                                params: { id: item.id }  // TODO: Implement page number.
                            })}
                            sx={{
                                cursor: 'pointer', // Show pointer cursor
                                width: '100%', // Ensure full width
                                '&:hover': {
                                    backgroundColor: 'action.hover' // Add hover effect
                                }
                            }}
                        >
                            <ListItemText
                                primary={item.name}
                                secondary={item.previewFields.join(" • ")}
                            />
                        </ListItem>
                        {index < items.length - 1 && <Divider />}
                    </React.Fragment>
                })}
            </List>
        </>
    );
};

export default ItemList;