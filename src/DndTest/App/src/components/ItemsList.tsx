import type { SearchHit } from "@/hooks/api/responses";
import { Divider, List, ListItem, ListItemText, Typography } from "@mui/material";
import { useNavigate } from "@tanstack/react-router";
import React from "react";

const ItemList = ({ hits }: { hits: Array<SearchHit> }) => {
    const navigate = useNavigate();

    return (
        <>
            <List>
                {hits.map((hit, index) => (
                    <React.Fragment key={hit.documentId + '-' + hit.pageNumber}>
                        <ListItem
                            onClick={() => navigate({
                                to: '/document/$id',
                                params: { id: hit.documentId }
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
                                primary={hit.name}
                                secondary={hit.category}
                            />
                        </ListItem>
                        {index < hits.length - 1 && <Divider />}
                    </React.Fragment>
                ))}
            </List>
        </>
    );
};

export default ItemList;