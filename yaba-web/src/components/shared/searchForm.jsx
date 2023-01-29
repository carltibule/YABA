import React from "react";
import { Button, Form } from "../external";

export function SearchForm(props) {
    return (
        <>
            <Form 
                onSubmit={props.onHandleSearch} 
                className="d-flex">
                    <Form.Control 
                        type="text"
                        className="me-2"
                        defaultValue={props.searchString}
                        onChange={props.onSearchStringChange}
                    />
                    <Button 
                        variant="primary"
                        type="submit"
                    >
                        { props.searchButtonText ?? "Search" }
                    </Button>
            </Form>
        </>
    );
}