import React from "react";
import { Button } from "../external";
import { getHighlightedText } from "../../utils";

export function Tag(props) {
    const getHighlightedTagName = () => props.searchString ? getHighlightedText(props.tag.name, props.searchString) : props.tag.name

    return (
        <Button
            variant="link" 
            style={{textDecoration: "none"}} className={`ms-0 me-2 p-0 ${props.tag.isHidden ? "text-danger" : null}`} 
            onClick={props.onClick}>
            #{getHighlightedTagName()}
        </Button>
    );
};