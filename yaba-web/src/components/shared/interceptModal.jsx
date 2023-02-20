import React from "react";
import { Button, Form, Modal } from "../external";

export function InterceptModal(props) {
    return (
        <Modal
            show={props.show}
            onHide={props.onHide}
            keyboard={false}
            backdrop={props.backdrop}
        >
            <Modal.Header closeButton>
                <Modal.Title>{props.title}</Modal.Title>
            </Modal.Header>
            <Modal.Body>{props.message}</Modal.Body>
            <Modal.Footer>
                <Button
                    variant={props.secondaryAction.variant}
                    onClick={props.secondaryAction.onClick}
                >
                    {props.secondaryAction.text}
                </Button>
                <Button
                    variant={props.primaryAction.variant}
                    onClick={props.primaryAction.onClick}
                >
                    {props.primaryAction.text}
                </Button>
            </Modal.Footer>
        </Modal>
    );
};
