import React, { useEffect, useReducer } from "react";
import { useNavigate, useParams } from "react-router-dom"; 
import { useFormik } from "formik";
import * as Yup from "yup"
import { SplashScreen } from "../components";
import { useAuth0 } from "@auth0/auth0-react";
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import { getBookmark, createNewBookmark, updateBookmark } from "../api";
import { Alert } from "react-bootstrap";

export function BookmarkDetailView() {
    const navigate = useNavigate();
    const { id } = useParams();
    const { getAccessTokenSilently } = useAuth0();
    
    const initialSplashScreenState = { show: false, message: null }
    
    const splashScreenReducer = (state = initialSplashScreenState, action) => {
        switch(action.type) {
            case "SHOW_SPLASH_SCREEN":
                return Object.assign({}, state, {
                    show: true,
                    message: action.message
                });
            case "HIDE_SPLASH_SCREEN":
            default:
                return Object.assign({}, state, {
                    show: false,
                    message: null
                });
        }
    };

    const [splashScreenState, dispatchSplashScreenState] = useReducer(splashScreenReducer, initialSplashScreenState);


    const initialBookmarkFormStateValues = {
        id: 0,
        createdOn: "",
        lastModified: null,
        title: "",
        description: "",
        note: "",
        isHidden: false,
        url: "",
        tags: ""
    }

    const bookmarkFormReducer = (state = initialBookmarkFormStateValues, action) => {
        switch(action.type) {
            case "FULL_UPDATE":
                return Object.assign({}, state, action.updatedData);
            default:
                return state;   
        }
    };

    const [bookmarkFormState, dispatchBookmarkFormState] = useReducer(bookmarkFormReducer, initialBookmarkFormStateValues);
    
    const alertMessageInitialState = { show: false, type: "primary", message: null};
    
    const alertReducer = (state = alertMessageInitialState, action) => {
        switch(action.type) {
            case "INSERT_FAIL":
                return Object.assign({}, state, {
                    show: true,
                    type: "danger",
                    message: `Error inserting new Bookmark record: ${action.message}`
                });
            case "UPDATE_FAIL":
                return Object.assign({}, state, {
                    show: true,
                    type: "danger",
                    message: `Error updating Bookmark record: ${action.message}`
                });
            case "INSERT_SUCCESSFUL":
                return Object.assign({}, state, {
                    show: true,
                    type: "success",
                    message: "New Bookmark entry created"
                });
            case "UPDATE_SUCCESSFUL":
                return Object.assign({}, state, {
                    show: true,
                    type: "success",
                    message: "Bookmark updated"
                });
            default:
                return Object.assign({}, state, {
                    show: false
                });
        }
    };

    const [alertMessageState, dispatchAlertMessageState] = useReducer(alertReducer, alertMessageInitialState);

    const formik = useFormik({
        initialValues: bookmarkFormState,
        validationSchema: Yup.object({
            url: Yup.string().required("Url is required")
        }),
        enableReinitialize: true,
        onSubmit: async(values) => {
            const isUpdate = values.id > 0
            values.tags = values.tags ? values.tags.toLowerCase().split(/[\s,]+/) : null;
            dispatchAlertMessageState({ type: "CLOSE_ALERT"});
            dispatchSplashScreenState({type: "SHOW_SPLASH_SCREEN", message: isUpdate ? "Updating Bookmark entry" : "Creating new Bookmark"});
            const accessToken = await getAccessTokenSilently();
            const { data, error } = isUpdate ?
                await updateBookmark(accessToken, values.id, values) : 
                await createNewBookmark(accessToken, values);

            if(error) {
                dispatchAlertMessageState({ type: isUpdate ? "UPDATE_FAIL" : "INSERT_FAIL", message: error.message });
            } else {
                dispatchAlertMessageState({ type: isUpdate ? "UPDATE_SUCCESSFUL" : "INSERT_SUCCESSFUL", message: null });
                dispatchBookmarkFormState({ type: "FULL_UPDATE", updatedData : data });
            }
            
            dispatchSplashScreenState({type: "HIDE_SPLASH_SCREEN", message: null });
        }
    });

    const handleOnCancelClick = () => {
        navigate("/bookmarks");    
    };

    const fetchBookmarkData = async () => {
        const accessToken = await getAccessTokenSilently();
        const { data, error } = await getBookmark(accessToken, id);

        if(error) {
            navigate("/404");
        } else {
            dispatchBookmarkFormState({type: "FULL_UPDATE", updatedData: {
                id: data.id ?? 0,
                createdOn: data.createdOn ?? "",
                lastModified: data.lastModified ?? "",
                title: data.title ?? "",
                description: data.description ?? "",
                note: data.note ?? "",
                isHidden: data.isHidden ?? false,
                url: data.url ?? "",
                tags: data.tags.map((x) => x.name).join(", ") ?? ""
            }});
            dispatchSplashScreenState({type: "HIDE_SPLASH_SCREEN", message: null });         
        }
    };

    useEffect(() => {
        if(id) {
            dispatchSplashScreenState({type: "SHOW_SPLASH_SCREEN", message: "Fetching Bookmark data"});
            fetchBookmarkData();    
        }
    }, [id]);
    

    return (
        <React.Fragment>
            {splashScreenState.show && (
                <SplashScreen 
                    message={splashScreenState.message}
                />
            )}

            <div style={{flexGrow: 1}}>
                <Container>
                    {alertMessageState.show && (
                        <Alert 
                            variant={alertMessageState.type} 
                            onClose={() => dispatchAlertMessageState({type: 'CLOSE'})}
                            dismissible
                        >
                            {alertMessageState.message}
                        </Alert>
                    )}
                    <Row>
                        <Col xs="9" className="mb-3">
                            { bookmarkFormState.id > 0 ?  <h1>Update Bookmark</h1> : <h1>Create new Bookmark</h1> }
                        </Col>
                        <Col xs="9">
                        <Form onSubmit={formik.handleSubmit}>
                            <Form.Group className="mb-3">
                                <Form.Label>Url:</Form.Label>
                                <Form.Control
                                    id="url"
                                    type="url" 
                                    placeholder="Enter Bookmark Url"
                                    defaultValue={formik.values.url}
                                    onBlur={formik.handleChange}
                                    isInvalid={!!formik.errors.url} 
                                />
                                <Form.Control.Feedback 
                                    type="invalid" 
                                    className="d-flex justify-content-start"
                                >
                                    {formik.errors.url}
                                </Form.Control.Feedback>
                            </Form.Group>
                            <Form.Group className="mb-3">
                                <Form.Label>Tags:</Form.Label>
                                <Form.Control 
                                    id="tags"
                                    type="text" 
                                    placeholder="Enter Bookmark Tags"
                                    defaultValue={formik.values.tags}
                                    onChange={formik.handleChange}
                                />
                                <Form.Text muted>
                                    Tags will be separated by commas or spaces
                                </Form.Text>
                            </Form.Group>
                            <Form.Group className="mb-3">
                                <Form.Label>Title:</Form.Label>
                                <Form.Control 
                                    id="title"
                                    type="text" 
                                    placeholder="Enter Bookmark Title"
                                    defaultValue={formik.values.title}
                                    onChange={formik.handleChange}
                                />
                                <Form.Text muted>
                                    When left blank, we'll use the URL's Title
                                </Form.Text>
                            </Form.Group>
                            <Form.Group className="mb-3">
                                <Form.Label>Description:</Form.Label>
                                <Form.Control 
                                    id="description"
                                    type="text" 
                                    placeholder="Enter Bookmark Description"
                                    defaultValue={formik.values.description}
                                    onChange={formik.handleChange}
                                />
                                <Form.Text className="text-muted">
                                    When left blank, we'll use the URL's Description
                                </Form.Text>
                            </Form.Group>
                            <Form.Group className="mb-3">
                                <Form.Label>Notes:</Form.Label>
                                <Form.Control 
                                    id="note"
                                    as="textarea" 
                                    rows={3} 
                                    placeholder="Enter notes"
                                    defaultValue={formik.values.note}
                                    onChange={formik.handleChange}
                                />
                            </Form.Group>
                            <Form.Group className="mb-3">
                                <Form.Check 
                                    id="isHidden"
                                    type="checkbox" 
                                    label="Mark as hidden"
                                    defaultValue={formik.values.isHidden}
                                    onChange={formik.handleChange}
                                />
                            </Form.Group>
                            <Button variant="primary" type="submit">
                                Submit
                            </Button>
                            <Button variant="link" type="submit" onClick={handleOnCancelClick}>
                                Cancel
                            </Button>
                        </Form>
                            
                        </Col>
                    </Row>
                </Container>
            </div>
        </React.Fragment>
    )
}