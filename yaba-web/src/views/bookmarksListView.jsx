import React, { useEffect, useReducer, useState } from "react";
import { Alert, Col, Container, Row, Button, Modal } from "../components/external";
import { Bookmark } from "../components";
import { useAuth0 } from "@auth0/auth0-react";
import { deleteBookmarks, getAllBookmarks, hideBookmarks } from "../api";
import { SplashScreen, SearchForm } from "../components";
import { getTagGroups, isSubset, containsSubstring, flattenTagArrays } from "../utils";

export function BookmarksListView(props) {
    const { getAccessTokenSilently } = useAuth0();
    const [searchString, setSearchString] = useState("");

    const handleSearch = (e) => {
        e.preventDefault();

        if(!searchString) {
            dispatchBookmarksState({type: "DISPLAY_ALL"});  
        } else {
            dispatchBookmarksState({type: "SEARCH"});
        }
    };

    const handleSearchStringChange = (e) => {
        if(!e.target.value) {
            dispatchBookmarksState({type: "DISPLAY_ALL"});
        } else {
            setSearchString(e.target.value);
        }
    };
    
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
            case "HIDE_ALERT":
            default:
                return Object.assign({}, state, {
                    show: false
                });
        }
    };
    const [alertMessageState, dispatchAlertMessageState] = useReducer(alertReducer, alertMessageInitialState);

    const bookmarksReducer = (state = [], action) => {
        switch(action.type) {
            case "SET":
                return action.payload.bookmarks.map(x => ({...x, isSelected: false, isDisplayed: true}));
            case "ADD_SELECTED":
            case "REMOVE_SELECTED":
                const modifiedBookmarks = [...state];
                const modifiedBookmarkIndex = modifiedBookmarks.findIndex((x) => x.id === action.payload.selectedBookmarkId);
                modifiedBookmarks[modifiedBookmarkIndex].isSelected = action.type === "ADD_SELECTED";
                return modifiedBookmarks;
            case "DELETE_SELECTED":
            case "HIDE_SELECTED":
                return state.filter((x) => !action.payload.selectedBookmarkIds.includes(x.id));
            case "SELECT_ALL":
                return state.map(x => ({...x, isSelected: true}));
            case "UNSELECT_ALL":
                return state.map(x => ({...x, isSelected: false}));
            case "DISPLAY_ALL":
                return state.map(x => ({...x, isDisplayed: true}));
            case "SEARCH":
                if(!searchString) {
                    dispatchBookmarksState({type: "DISPLAY_ALL"});
                }
                
                return state.map(x => ({...x, isDisplayed: containsSubstring(x, searchString)}));
            default:
                return state;
        }
    };

    const [bookmarksState, dispatchBookmarksState] = useReducer(bookmarksReducer, []);
    const onBookmarkSelected = (isBookmarkSelected, bookmark) => dispatchBookmarksState({type: isBookmarkSelected ?  "ADD_SELECTED" : "REMOVE_SELECTED", payload: {selectedBookmarkId: bookmark.id}});
    const getSelectedBookmarksCount = () => bookmarksState.filter((x) => x.isSelected).length;
    const getSelectedBookmarks = () => bookmarksState.filter((x) => x.isSelected);
    const getAreAllBookmarksSelected = () => bookmarksState.every(x => x.isSelected);
    const getFilteredBookmarks = () => {
        if (getSelectedTags().length <= 0) {
            return bookmarksState.filter(x => x.isDisplayed);
        } else {
            return bookmarksState.filter(x => isSubset(x.tags.map(x => x.id), getSelectedTags().map(x => x.id)));
        }
    }

    const onDeleteSelectedBookmarks = async (ids) => {
        if(ids.length <= 0) {
            dispatchAlertMessageState({type: "SHOW_ALERT", payload: {show: true, alertType: "warning", message: "No bookmark(s) to delete"}});
        } else {
            dispatchSplashScreenState({type: "SHOW_SPLASH_SCREEN", payload: {message: "Deleting bookmark(s)"}});
            const accessToken = await getAccessTokenSilently();
            const { data, error } = await deleteBookmarks(accessToken, ids);

            if(error) {
                dispatchAlertMessageState({type: "SHOW_ALERT", payload: {show: true, alertType: "danger", message: `An error has occurred while deleting bookmark(s): ${error.message}`}})
            } else {
                dispatchBookmarksState({type: "DELETE_SELECTED", payload: {selectedBookmarkIds: ids}});
            }
            dispatchSplashScreenState({type: "HIDE_SPLASH_SCREEN", payload: {message: null }});
        }
    };

    const initialConfirmModalState = {show: false, title: null, message: null, selectedBookmarkId: null};

    const confirmModalReducer = (state = initialConfirmModalState, action) => {
        switch(action.type) {
            case "SHOW_FOR_MULTIPLE_BOOKMARK_DELETE":
                return {
                    ...state,
                    show: action.payload.show,
                    title: action.payload.title,
                    message: action.payload.message
                };
            case "SHOW_FOR_BOOKMARK_DELETE":
                return {
                    ...state,
                    show: action.payload.show,
                    title: action.payload.title,
                    message: action.payload.message,
                    selectedBookmarkId: action.payload.selectedBookmarkId
                };
            case "HIDE":
            default: 
                return initialConfirmModalState;
        }
    };

    const [confirmModalState, dispatchConfirmModalState] = useReducer(confirmModalReducer, initialConfirmModalState);
    const handleDeleteMultipleBookmarks = () => dispatchConfirmModalState({"type": "SHOW_FOR_MULTIPLE_BOOKMARK_DELETE", payload: {show: true, title: "Delete selected bookmarks", "message": "Are you sure you want to delete selected bookmarks?"}});
    const handleDeleteBookmark = (bookmarkId) => dispatchConfirmModalState({type: "SHOW_FOR_BOOKMARK_DELETE", payload: { show: true, title: "Delete bookmark", message: "Are you sure you want to delete this bookmark?", selectedBookmarkId: bookmarkId}});

    const onHandleCloseConfirmModal = async () => {
        const bookmarkIdsToDelete = confirmModalState.selectedBookmarkId ? [confirmModalState.selectedBookmarkId] : getSelectedBookmarks().map((x) => x.id);
        dispatchConfirmModalState({type: "HIDE"});
        await onDeleteSelectedBookmarks(bookmarkIdsToDelete);
    };

    const handleHideBookmarks = async(ids) => {
        if(ids.length <= 0) {
            dispatchAlertMessageState({type: "SHOW_ALERT", payload: {show: true, alertType: "warning", message: "No bookmark(s) to hide"}});
        } else {
            dispatchSplashScreenState({type: "SHOW_SPLASH_SCREEN", payload: {message: "Hiding bookmark(s)"}});
            const accessToken = await getAccessTokenSilently();
            const { data, error } = await hideBookmarks(accessToken, ids);

            if(error) {
                dispatchAlertMessageState({type: "SHOW_ALERT", payload: {show: true, alertType: "danger", message: `An error has occurred while hiding bookmark(s): ${error.message}`}})
            } else {
                dispatchBookmarksState({type: "HIDE_SELECTED", payload: {selectedBookmarkIds: ids}});
            }
            dispatchSplashScreenState({type: "HIDE_SPLASH_SCREEN", payload: {message: null }});
        }
        
    };

    const fetchBookmarks = async() => {
        const accessToken = await getAccessTokenSilently();
        const { data, error } = await getAllBookmarks(accessToken, props.showHidden);

        if(error) {
            dispatchAlertMessageState({type: "SHOW_ALERT", payload: {show: true, alertType: "danger", "message": `Error fetching bookmarks: ${error.message}`}});
        } else {
            dispatchBookmarksState({type: "SET", payload: {bookmarks: data}});
            dispatchTagsState({type: "SET"});
        }
    }

    
    const tagsReducer = (state = [], action) => {
        switch(action.type) {
            case "SET":
                return flattenTagArrays(bookmarksState.map(x => x.tags)).map(x => Object.assign({}, x, {isSelected : false}));
            case "ADD_SELECTED":
            case "REMOVE_SELECTED":
                const modifiedTags = [...state];
                const selectedTagIndex = modifiedTags.findIndex((x) => x.id === action.payload.selectedTagId);
                modifiedTags[selectedTagIndex].isSelected = action.type === "ADD_SELECTED";
                return modifiedTags;
            default:
                return state;
        }
    }
    
    const [tagsState, dispatchTagsState] = useReducer(tagsReducer, []);
    const onTagSelected = (isTagSelected, tag) => dispatchTagsState({type: isTagSelected ? "ADD_SELECTED" : "REMOVE_SELECTED", payload: {selectedTagId: tag.id}})
    const getSelectedTags = () => tagsState.filter((x) => x.isSelected);
    const getNotSelectedTags = () => tagsState.filter((x) => !x.isSelected);

    useEffect(() => {
        dispatchSplashScreenState({type: "SHOW_SPLASH_SCREEN", payload: {message: "Retrieving Bookmarks..."}});
        fetchBookmarks();

        dispatchSplashScreenState({type: "HIDE_SPLASH_SCREEN", payload: {message: null}});
    }, []);
    
    return(
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
                            onClose={() => dispatchAlertMessageState({type: "CLOSE"})}
                            dismissible
                        >
                            {alertMessageState.message}
                        </Alert>
                    )}
                    <Modal
                        show={confirmModalState.show}
                        onHide={() => {dispatchConfirmModalState({type: "HIDE"})}}
                        keyboard={false}
                    >
                        <Modal.Header closeButton>
                            <Modal.Title>{confirmModalState.title}</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>{confirmModalState.message}</Modal.Body>
                        <Modal.Footer>
                            <Button 
                                variant="secondary"
                                onClick={() => {dispatchConfirmModalState({type: "HIDE"})}}
                            >
                                Cancel
                            </Button>
                            <Button 
                                variant="danger"
                                onClick={onHandleCloseConfirmModal}
                            >
                                Delete
                            </Button>
                        </Modal.Footer>
                    </Modal>
                    <Row>
                        <Col xs="9">
                            <Row>
                                <Col xs="7">
                                    <span className="fs-4">{props.showHidden ? "Hidden Bookmarks" : "Bookmarks"}</span>
                                </Col>
                                <Col xs="5" className="d-flex justify-content-end">
                                    <SearchForm 
                                        onHandleSearch={handleSearch}
                                        onSearchStringChange={handleSearchStringChange}
                                        searchString={searchString}
                                    />
                                </Col>
                            </Row>
                            <hr className="mt-1" />
                        </Col>
                        <Col xs="3">
                            <span className="fs-4">Tags</span>
                            <hr className="mt-1" />
                        </Col>
                    </Row>
                    <Row>
                        <Col xs="2" className="mb-3">
                            {
                                bookmarksState.length > 0 &&
                                    <Button variant="primary" onClick={() => dispatchBookmarksState({type: getAreAllBookmarksSelected() ? "UNSELECT_ALL" : "SELECT_ALL"})}>{getAreAllBookmarksSelected() ? "Unselect All" : "Select All" }</Button>
                            }
                        </Col>
                        <Col xs="7" className="mb-3">
                            {
                                getSelectedBookmarksCount() > 0 && 
                                    <div className="d-flex justify-content-end align-items-center">
                                        <span className="fs-5 me-2"> {getSelectedBookmarksCount()} selected</span>
                                        <Button variant="primary" className="me-2" onClick={() => handleHideBookmarks(getSelectedBookmarks().map(x => x.id))}>{props.showHidden ? "Unhide" : "Hide"}</Button>
                                        <Button 
                                            variant="danger"
                                            onClick={handleDeleteMultipleBookmarks}
                                        >
                                            Delete
                                        </Button>
                                    </div>
                            }
                        </Col>
                        <Col xs="3" className="mb-3">
                            { 
                                getSelectedTags().length > 0 && (
                                    getSelectedTags().map((tag) => {
                                        return <Button key={tag.id} variant="primary" style={{textDecoration: "none", cursor: "pointer" }}  className="badge rounded-pill text-bg-primary me-2" onClick={() => onTagSelected(false, tag)}>#{tag.name} | x</Button>
                                    })
                                )
                            }
                        </Col>
                    </Row>
                    <Row>
                        <Col xs="9">
                            {
                                getFilteredBookmarks().map((bookmark) => {
                                    return <Bookmark 
                                        key={bookmark.id} 
                                        bookmark={bookmark} 
                                        onBookmarkSelected={(selected) => onBookmarkSelected(selected, bookmark)}
                                        onDeleteClicked={() => handleDeleteBookmark(bookmark.id)}
                                        onHideClicked={() => handleHideBookmarks([bookmark.id])}
                                    />
                                })
                            }
                        </Col>
                        <Col xs="3">
                            {
                                getTagGroups(getNotSelectedTags()).map((group) => {
                                    return <div key={group.name} className="mb-3">
                                            <span key={group.name} className="text-primary fw-bold">{group.name}</span>
                                            <br />
                                            {
                                                group.tags.map((tag) => {
                                                    return <Button key={tag.id} variant="link" style={{textDecoration: "none"}} className={`ms-0 me-2 p-0 ${tag.isHidden ? "text-danger" : null}`} onClick={() => onTagSelected(true, tag)}>
                                                            #{tag.name}
                                                        </Button>
                                                })
                                            }
                                        </div>
                                })
                            }
                        </Col>
                    </Row>
                </Container> 
            </div>
        </React.Fragment>
    );
}