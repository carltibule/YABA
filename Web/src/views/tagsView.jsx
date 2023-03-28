import React, { useEffect, useReducer, useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { containsSubstring, getHighlightedText } from "../utils";
import { SplashScreen, SearchForm, UpsertTagModal, InterceptModal } from "../components";
import { Alert, Button, Col, Container, Dropdown, DropdownButton, Form, Row, Table } from "../components/external";
import { getAllTags, createNewTag, updateTag, deleteTags, hideTags } from "../api";

export function TagsView(props) {
    const { getAccessTokenSilently } = useAuth0();

    const [searchString, setSearchString] = useState("");
    const [isAllTagsSelected, setAllTagsSelected] = useState(false);

    const initialSplashScreenState = { show: false, message: null };
    const splashScreenReducer = (state = initialSplashScreenState, action) => {
        switch(action.type) {
            case "SHOW_SPLASH_SCREEN":
                return Object.assign({}, state, {
                    show: true,
                    message: action.payload.message
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

    const alertMessageInitialState = { show: false, type: "primary", message: null};
    const alertReducer = (state = alertMessageInitialState, action) => {
        switch(action.type) {
            case "SHOW_ALERT":
                return Object.assign({}, state, {
                    show: action.payload.show,
                    type: action.payload.alertType,
                    message: action.payload.message
                });
            case "UPSERT_FAIL":
                return {
                    show: true,
                    type: "danger",
                    message: action.payload.message
                };
            case "UPSERT_SUCCEEDED":
                return {
                    show: true,
                    type: "success",
                    message: action.payload.message
                };
            case "NO_TAG_TO_DELETE":
                return {
                    show: true,
                    type: "warning",
                    message: "No tag(s) to delete"
                };
            case "DELETE_FAILED":
                return {
                    show: true,
                    type: "danger",
                    message: action.payload.message
                };
            case "DELETE_SUCCEEDED":
                return {
                    show: true,
                    type: "success",
                    message: "Tag(s) deleted"
                }
            case "NO_TAG_TO_HIDE":
                return {
                    show: true,
                    type: "warning",
                    message: "No tag(s) to hide/unhide"
                }
            case "HIDE_ALERT":
            default:
                return Object.assign({}, state, {
                    show: false
                });
        }
    };
    const [alertMessageState, dispatchAlertMessageState] = useReducer(alertReducer, alertMessageInitialState);

    const tagsReducer = (state = [], action) => {
        let newState = [];

        switch(action.type) {
            case "SET":
                newState = action.payload.tags.map(x => ({...x, isSelected: false, isDisplayed: true}));
                break;
            case "ADD_SELECTED":
            case "REMOVE_SELECTED":
                newState = [...state];
                const selectedTagIndex = newState.findIndex(x => x.id === action.payload.selectedTagId);
                newState[selectedTagIndex].isSelected = action.type === "ADD_SELECTED";
                break;
            case "DELETE_SELECTED":
                newState = state.filter((x) => !action.payload.selectedTagIds.includes(x.id));
                break;
            case "SELECT_ALL":
                newState = state.filter(x => x.isDisplayed).map(x => ({...x, isSelected: true}));
                break;
            case "UNSELECT_ALL":
                newState = state.filter(x => x.isDisplayed).map(x => ({...x, isSelected: false}));
                break;
            case "DISPLAY_ALL":
                newState = state.map(x => ({...x, isDisplayed: true}));
                break;
            case "SEARCH":
                newState = state.map(x => ({...x, isDisplayed: !searchString || containsSubstring(x, searchString)}));
                break;
            case "TAG_UPDATE":
                newState = [...state];
                const updatedTagIndex = newState.findIndex(x => x.id == action.payload.tag.id);
                newState.splice(updatedTagIndex, 1, Object.assign({}, newState[updatedTagIndex], action.payload.tag));
                break;
            case "TAG_INSERT": 
                newState = [{...action.payload.tag, isDisplayed: true, isSelected: false}, ...state];
                break;
            case "HIDE_SUCCEEDED":
                newState = [...state.filter(x => !x.isSelected), ...state.filter(x => x.isSelected).map(x => Object.assign({}, x, {isHidden: !x.isHidden}))];
                break;
            default:
                newState = state;
                break;
        }

        setAllTagsSelected(newState.every(x => x.isSelected));
        return newState;
    };
    const [tagsState, dispatchTagsState] = useReducer(tagsReducer, []);
    const getTagsSelectedCount = () => tagsState.filter(x => x.isSelected).length;
    const onTagSelected = (isTagSelected, tag) => dispatchTagsState({type: isTagSelected ? "ADD_SELECTED" : "REMOVE_SELECTED", payload: {selectedTagId: tag.id}});
    const getDisplayedTags = () => tagsState.filter(tag => tag.isDisplayed);
    const getSelectedTags = () => tagsState.filter(tag => tag.isSelected);
    const isAllSelectedHidden = () => getSelectedTags().every(tag => tag.isHidden);
    const isAllSelectedNotHidden = () => getSelectedTags().every(tag => !tag.isHidden);

    const upsertTagModalInitialState = { show: false, tag: { id: 0, name: "", isHidden: false }};
    const upsertTagModalReducer = (state = upsertTagModalInitialState, action) => {
        switch(action.type) {
            case "SHOW_EDIT":
                return {show: true, tag: action.payload.tag};
            case "SHOW_NEW":
                return {...upsertTagModalInitialState, show: true};
            case "HIDE":
            default:
                return upsertTagModalInitialState;
        }
    }
    const [upsertTagModalState, dispatchUpsertTagModal] = useReducer(upsertTagModalReducer, upsertTagModalInitialState);

    const initialInterceptModalState = {
            show: false,
            backdrop: "static",
            title: null, 
            message: null,
            secondaryAction: {
                variant: "secondary",
                text: "Cancel",
                onClick: null,
            },
            primaryAction: {
                variant: "danger",
                text: "Delete",
                onClick: null,
            },
            selectedTagIds: [] 
    };
    const interceptModalReducer = (state = initialInterceptModalState, action) => {
        switch(action.type) {
            case "SHOW_FOR_MULTIPLE_DELETE":
                return {
                    ...state,
                    show: true,
                    title: "Delete Tags",
                    message: "Are you sure you want to delete selected tags?",
                    secondaryAction: {
                        variant: "secondary",
                        text: "Cancel",
                        onClick: null,
                    },
                    primaryAction: {
                        variant: "danger",
                        text: "Delete",
                        onClick: null,
                    },
                    selectedTagIds: action.payload.selectedTagIds,
                };
            case "SHOW_FOR_DELETE":
                return {
                    ...state,
                    show: true,
                    title: "Delete Tag",
                    message: "Are you sure you want to delete this tag?",
                    secondaryAction: {
                        variant: "secondary",
                        text: "Cancel",
                        onClick: null,
                    },
                    primaryAction: {
                        variant: "danger",
                        text: "Delete",
                        onClick: null,
                    },
                    selectedTagIds: action.payload.selectedTagIds,
                };
            case "HIDE":
            default:
                return initialInterceptModalState;
        }
    };
    const [interceptModalState, dispatchInterceptModalState] = useReducer(interceptModalReducer, initialInterceptModalState);

    const fetchTags = async() => {
        const accessToken = await getAccessTokenSilently();
        const { data, error } = await getAllTags(accessToken);

        if(error) {
            dispatchAlertMessageState({type: "SHOW_ALERT", payload: {show: true, alertType: "danger", "message": `Error fetching tags: ${error.message}`}});
        } else {
            dispatchTagsState({type: "SET", payload: {tags: data}});
        }
        dispatchSplashScreenState({type: "HIDE_SPLASH_SCREEN", payload: {message: null}});
    };

    const handleUpsertTag = async(tagEntry) => {
        const isUpdate = tagEntry.id > 0;
        dispatchAlertMessageState({ type: "CLOSE_ALERT"});
        dispatchUpsertTagModal({type: "HIDE"});
        dispatchSplashScreenState({type: "SHOW_SPLASH_SCREEN", payload: {message: isUpdate ? "Updating Bookmark entry" : "Creating new Bookmark"}});

        const accessToken = await getAccessTokenSilently();
        const { data, error } = isUpdate ? 
            await updateTag(accessToken, tagEntry.id, tagEntry) :
            await createNewTag(accessToken, tagEntry);
        
            if(error) {
                dispatchAlertMessageState({type: "UPSERT_FAIL", payload: {message: `Error ${isUpdate ? "updating": "inserting new"} Tag record: ${error.message}`}});
            } else {
                dispatchAlertMessageState({type: "UPSERT_SUCCEEDED", payload: {message: isUpdate ? "Tag updated" : "Tag created"}});
                dispatchTagsState({ type: isUpdate? "TAG_UPDATE" : "TAG_INSERT", payload: {tag: data}});
            }

        dispatchSplashScreenState({type: "HIDE_SPLASH_SCREEN", payload: {message: null}});
    };

    const handleDeleteTags = async () => {
        dispatchInterceptModalState({type: "HIDE"});
        dispatchUpsertTagModal({type: "HIDE"})
        const tagIdsToDelete = interceptModalState.selectedTagIds;

        if(tagIdsToDelete.length <= 0) {
            dispatchAlertMessageState({type: "NO_TAG_TO_DELETE"})
        } else {
            dispatchSplashScreenState({type: "SHOW_SPLASH_SCREEN", payload: {message: "Deleting Tag(s)"}});
            const accessToken = await getAccessTokenSilently();
            const { data, error } = await deleteTags(accessToken, tagIdsToDelete);

            if(error) {
                dispatchAlertMessageState({type: "DELETE_FAILED", payload: {message: `Error deleting tag(s): ${error.message}`}});
            } else {
                dispatchAlertMessageState({type: "DELETE_SUCCEEDED"});
                dispatchTagsState({type: "DELETE_SELECTED", payload: {selectedTagIds: tagIdsToDelete}});
            }

            dispatchSplashScreenState({TYPE: "HIDE_SPLASH_SCREEN", payload: {message: null}});
        }
    };

    const handleHidingTags = async () => {
        const tagIdsToHide = getSelectedTags().map(x => x.id);

        if(tagIdsToHide.length <= 0) {
            dispatchAlertMessageState({type: "NO_TAG_TO_HIDE"})
        } else {
            dispatchSplashScreenState({type: "SHOW_SPLASH_SCREEN", payload: {message: "Hiding/Unhiding Tag(s)"}});
            const accessToken = await getAccessTokenSilently();
            const { data, error } = await hideTags(accessToken, tagIdsToHide);

            if(error) {
                dispatchAlertMessageState({type: "UPSERT_FAILED", payload: {message: `Error hiding/unhiding tag(s): ${error.message}`}}); 
            } else {
                dispatchAlertMessageState({type: "UPSERT_SUCCEEDED", payload: {message: `Tag(s) have been hidden/unhidden`}});
                dispatchTagsState({type: "HIDE_SUCCEEDED"});
                dispatchTagsState({type: "UNSELECT_ALL"});
            }

            dispatchSplashScreenState({TYPE: "HIDE_SPLASH_SCREEN", payload: {message: null}});
        }
    };

    const handleSearch = (e) => {
        e.preventDefault();
        setSearchString(e.target[0].value);
        dispatchTagsState({type: "SEARCH"});
    };

    const handleSearchStringChange = (e) => {
        setSearchString(e.target.value);
        dispatchTagsState({type: "SEARCH"});
    };

    useEffect(() => {
        dispatchSplashScreenState({type: "SHOW_SPLASH_SCREEN", payload: {message: "Retrieving Tags..."}});
        fetchTags();
    }, []);

    return (
        <>
            {splashScreenState.show && (
                <SplashScreen 
                    message={splashScreenState.message}
                />
            )}

            <UpsertTagModal
                show={upsertTagModalState.show}
                tag={upsertTagModalState.tag} 
                onCancel={() => dispatchUpsertTagModal({type: "HIDE"})}
                onHide={() => dispatchUpsertTagModal({type: "HIDE"})}
                onSave={handleUpsertTag}
                onDelete={() => dispatchInterceptModalState({type: "SHOW_FOR_DELETE", payload: {selectedTagIds: [upsertTagModalState.tag.id]}})}
            />
            <InterceptModal 
                show={interceptModalState.show}
                onHide={() => dispatchInterceptModalState({type: "HIDE"})}
                backdrop={interceptModalState.backdrop}
                title={interceptModalState.title}
                message={interceptModalState.message}
                secondaryAction={{...interceptModalState.secondaryAction, onClick: () => dispatchInterceptModalState({type: "HIDE"})}}
                primaryAction={{...interceptModalState.primaryAction, onClick: () => handleDeleteTags()}}
            />

            <div style={{flexGrow: 1}}>
                <Container>
                    { alertMessageState.show && <Alert
                            variant={alertMessageState.type}
                            onClose={() => dispatchAlertMessageState({type: "CLOSE"})}
                            dismissible
                        >
                            {alertMessageState.message}
                        </Alert>
                    }
                    { tagsState.length <= 0 && !splashScreenState.show && <div className="text-center fs-1">No Tags found</div> }
                    {
                        tagsState.length > 0 && <>
                            <Row>
                                <Col xs="5">
                                    <SearchForm 
                                        onHandleSearch={handleSearch}
                                        onSearchStringChange={handleSearchStringChange}
                                        onBlur={handleSearchStringChange}
                                        searchString={searchString}
                                    />
                                </Col>
                                <Col xs="4">
                                    <div className="d-flex justify-content-end align-items-center">
                                        <Button variant="primary" className="me-2" onClick={() => dispatchUpsertTagModal({type: "SHOW_NEW"})}>New</Button>
                                        { getTagsSelectedCount() > 0 && <DropdownButton variant="secondary" title={`${getTagsSelectedCount()} selected`}>
                                                { isAllSelectedHidden() && <Dropdown.Item onClick={handleHidingTags}>Mark as Not Hidden</Dropdown.Item>}
                                                { isAllSelectedNotHidden() && <Dropdown.Item onClick={handleHidingTags}>Mark as Hidden</Dropdown.Item>}
                                                <Dropdown.Item onClick={() => dispatchInterceptModalState({type: "SHOW_FOR_MULTIPLE_DELETE", payload: {selectedTagIds: getSelectedTags().map(x => x.id)}})}>
                                                    <span className="text-danger">Delete</span>
                                                </Dropdown.Item>
                                            </DropdownButton>
                                        }
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col xs="9">
                                    <Table>
                                        <thead>
                                            <tr>
                                                <th>
                                                    <Form.Check 
                                                        type="checkbox"
                                                        onChange={(e) => dispatchTagsState({type: e.target.checked ? "SELECT_ALL" : "UNSELECT_ALL"})}
                                                        checked={isAllTagsSelected}
                                                    />
                                                </th>
                                                <th>Tag</th>
                                                <th>Is Hidden?</th>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {
                                                getDisplayedTags().map(tag => {
                                                    return <tr key={tag.id}>
                                                        <td>
                                                            <Form.Check 
                                                                type="checkbox"
                                                                onChange={(e) => onTagSelected(e.target.checked, tag)}
                                                                checked={tag.isSelected}
                                                            />
                                                        </td>
                                                        <td>{getHighlightedText(tag.name, searchString)}</td>
                                                        <td>
                                                            <span className={tag.isHidden ? "text-danger" : "text-dark"}>{ tag.isHidden ? "Yes" : "No" }</span>
                                                        </td>
                                                        <td>
                                                            <Button variant="link" className="py-0" onClick={() => dispatchUpsertTagModal({type: "SHOW_EDIT", payload: {tag: tag}})}>Edit</Button>
                                                        </td>
                                                    </tr>
                                                })
                                            }
                                        </tbody>
                                    </Table>
                                </Col>
                            </Row>
                        </>
                    }
                </Container>
            </div>
        </>
    )
};