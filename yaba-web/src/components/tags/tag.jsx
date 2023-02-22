import React from "react";
import { Button } from "../external";

export function Tag(props) {
    return (
        <Button
            variant="link" 
            style={{textDecoration: "none"}} className={`ms-0 me-2 p-0 ${props.tag.isHidden ? "text-danger" : null}`} 
            onClick={props.onClick}>
            #{props.tag.name}
        </Button>
    );
};