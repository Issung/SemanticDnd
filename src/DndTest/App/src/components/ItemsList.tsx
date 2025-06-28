import type { ItemSummary, SearchHit } from "@/hooks/api/responses";
import { ItemType } from "@/hooks/api/responses";
import FolderIcon from '@mui/icons-material/Folder';
import { Divider, List, ListItem, ListItemIcon, ListItemText } from "@mui/material";
import { useNavigate } from "@tanstack/react-router";
import React from "react";
import Navigations from "../Navigations";

export class ItemListDisplayAdapter {
    constructor(
        public id: number,
        public name: string,
        public type: ItemType,
        public previewFields: Array<string>,
        public pageNumber: number | undefined
    ) {}

    static fromSummary(item: ItemSummary): ItemListDisplayAdapter {
        return new ItemListDisplayAdapter(
            item.id,
            item.name,
            item.type,
            item.previewFields,
            undefined // No page number in ItemSummary
        );
    }

    static fromSearchHit(hit: SearchHit): ItemListDisplayAdapter {
        const a =  ItemListDisplayAdapter.fromSummary(hit.item);
        a.pageNumber = 5;
        return a;
    }
}


const ItemList = ({ hits: items }: { hits: Array<ItemListDisplayAdapter> }) => {
    const navigate = useNavigate();

    function handleClick(item: ItemListDisplayAdapter) {
        if (item.type == ItemType.Folder) {
            navigate(Navigations.browse(item.id));
        }
        else {
            navigate({
                to: '/item/$id',
                params: { id: item.id }  // TODO: Implement page number.
            })
        }
    }

    return (
        <>
            <List>
                {items.map((item, index) => {
                    const key = `${item.id}${item.pageNumber ? `-${item.pageNumber}` : ''}`;

                    return <React.Fragment key={key}>
                        <ListItem
                            onClick={() => handleClick(item)}
                            sx={{
                                cursor: 'pointer', // Show pointer cursor
                                width: '100%', // Ensure full width
                                '&:hover': {
                                    backgroundColor: 'action.hover' // Add hover effect
                                }
                            }}
                        >
                            {item.type === ItemType.Folder && 
                                <ListItemIcon>
                                    <FolderIcon/>
                                </ListItemIcon>
                            }
                            
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