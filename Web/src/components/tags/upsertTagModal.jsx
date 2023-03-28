import React from "react";
import { Button, Form, Modal } from "../external";
import { useFormik } from "formik";
import * as Yup from "yup"

export function UpsertTagModal(props) {
    const formik = useFormik({
        initialValues: props.tag,
        validationSchema: Yup.object({
            name: Yup.string().required("Name is required").lowercase()
        }),
        enableReinitialize: true,
        onSubmit: async(values) => props.onSave(values)
    });

    return (
        <Modal
            show={props.show}
            onHide={props.onHide}
            keyboard={false}
            backdrop="static"
        >
            <Form onSubmit={formik.handleSubmit}>
                <Modal.Header closeButton>
                    <Modal.Title>{props.tag.id > 0 ? "Edit Tag" : "New Tag"}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form.Group className="mb-3">
                    <Form.Label>Name:</Form.Label>
                    <Form.Control
                        id="name"
                        type="text"
                        placeholder="Enter tag name"
                        defaultValue={formik.values.name}
                        onBlur={formik.handleChange}
                        isInvalid={!!formik.errors.name}
                    />
                    <Form.Control.Feedback
                        type="invalid"
                        className="d-flex justify-content-start"
                    >
                        {formik.errors.name}
                    </Form.Control.Feedback>
                </Form.Group>
                <Form.Group>
                    <Form.Check
                        id="isHidden"
                        type="switch"
                        label="Mark as hidden"
                        checked={formik.values.isHidden}
                        onChange={formik.handleChange}
                    />
                </Form.Group>
                </Modal.Body>
                <Modal.Footer>
                    { props.tag.id > 0 && <Button
                            variant="danger"
                            onClick={props.onDelete}
                        >
                            Delete
                        </Button>
                    }
                    <Button
                        variant="primary"
                        type="submit"
                    >
                        Save
                    </Button>
                </Modal.Footer>
            </Form>
        </Modal>
    );
}