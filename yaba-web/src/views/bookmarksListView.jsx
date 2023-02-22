import React, { useEffect, useReducer, useState } from "react";
import { Alert, Col, Container, Row, Button, Modal, Dropdown, DropdownButton } from "../components/external";
import { Bookmark } from "../components";
import { useAuth0 } from "@auth0/auth0-react";
import { deleteBookmarks, getAllBookmarks, hideBookmarks } from "../api";
import { SplashScreen, SearchForm } from "../components";
import { getTagGroups, isSubset, containsSubstring, flattenTagArrays } from "../utils";

export function BookmarksListView(props) {
    const { getAccessTokenSilently } = useAuth0();
    const [searchString, setSearchString] = useState("");
    
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

    const tagsInitialState = [];

    const tagsReducer = (state = tagsInitialState, action) => {
        switch(action.type) {
            case "SET":
                return action.payload.tags.map(x => ({...x, isSelected: false, isDisplayed: true }));
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

    const [tagsState, dispatchTagsState] = useReducer(tagsReducer, tagsInitialState);
    const getSelectedTags = () => tagsState.filter((x) => x.isSelected);
    const getNotSelectedTags = () => tagsState.filter((x) => !x.isSelected);

    const bookmarksInitialState = [];

    const bookmarksReducer = (state = bookmarksInitialState, action) => {
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
                return state.map(x => ({...x, isSelected: x.isDisplayed}));
            case "UNSELECT_ALL":
                return state.map(x => ({...x, isSelected: false}));
            case "DISPLAY_ALL":
                return state.map(x => ({...x, isDisplayed: true}));
            case "SEARCH":
                return state.map(x => ({...x, isDisplayed: (getSelectedTags().length <= 0 || isSubset(x.tags.map(x => x.id), getSelectedTags().map(x => x.id)))
                    && (!searchString || containsSubstring(x, searchString))}));
            default:
                return state;
        }
    };

    const [bookmarksState, dispatchBookmarksState] = useReducer(bookmarksReducer, []);
    const onBookmarkSelected = (isBookmarkSelected, bookmark) => dispatchBookmarksState({type: isBookmarkSelected ?  "ADD_SELECTED" : "REMOVE_SELECTED", payload: {selectedBookmarkId: bookmark.id}});
    const getDisplayedBookmarksCount = () => bookmarksState.filter(x => x.isDisplayed).length;
    const getSelectedBookmarksCount = () => bookmarksState.filter(x => x.isSelected && x.isDisplayed).length;
    const getSelectedBookmarks = () => bookmarksState.filter((x) => x.isSelected && x.isDisplayed);
    const getAreAllBookmarksSelected = () => bookmarksState.filter(x => x.isDisplayed).every(x => x.isSelected);
    const getFilteredBookmarks = () => bookmarksState.filter(x => x.isDisplayed);

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

    const onTagSelected = (isTagSelected, tag) => {
        dispatchTagsState({type: isTagSelected ? "ADD_SELECTED" : "REMOVE_SELECTED", payload: {selectedTagId: tag.id}});
        dispatchBookmarksState({type: "SEARCH"});      
    };

    const handleSearch = (e) => {
        e.preventDefault();
        setSearchString(e.target[0].value);
        dispatchBookmarksState({type: "SEARCH"});
    };

    const handleSearchStringChange = (e) => {
        setSearchString(e.target.value);
        dispatchBookmarksState({type: "SEARCH"});
    };

    useEffect(() => {
        dispatchSplashScreenState({type: "SHOW_SPLASH_SCREEN", payload: {message: "Retrieving Bookmarks..."}});

        const fetchBookmarks = async() => {
            const accessToken = await getAccessTokenSilently();
            const { data, error } = await getAllBookmarks(accessToken, props.showHidden);
    
            if(error) {
                dispatchAlertMessageState({type: "SHOW_ALERT", payload: {show: true, alertType: "danger", "message": `Error fetching bookmarks: ${error.message}`}});
            } else {
                dispatchBookmarksState({type: "SET", payload: {bookmarks: data}});
                dispatchTagsState({type: "SET", payload: {tags: flattenTagArrays(data.map(x => x.tags))}});    
            }
    
            dispatchSplashScreenState({type: "HIDE_SPLASH_SCREEN", payload: {message: null}});
        }

        fetchBookmarks();
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
                        <Col xs="9" className="mb-3">
                            <div className="d-flex justify-content-start align-items-center">
                                {
                                    getDisplayedBookmarksCount() <= 0 &&
                                        <span className="fs-4">No bookmarks to display</span>
                                }

                                {
                                    getDisplayedBookmarksCount() > 0 && 
                                        <Button className="me-2" variant="primary" onClick={() => dispatchBookmarksState({type: getAreAllBookmarksSelected() ? "UNSELECT_ALL" : "SELECT_ALL"})}>{getAreAllBookmarksSelected() ? "Unselect All" : "Select All" }</Button>
                                }

                                {
                                    getSelectedBookmarksCount() > 0 && 
                                        <DropdownButton variant="secondary" title={`${getSelectedBookmarksCount()} selected`}>
                                            <Dropdown.Item onClick={() => handleHideBookmarks(getSelectedBookmarks().map(x => x.id))}>
                                                {props.showHidden ? "Unhide" : "Hide"}
                                            </Dropdown.Item>
                                            <Dropdown.Item onClick={handleDeleteMultipleBookmarks}>
                                            <span className="text-danger">Delete</span>
                                            </Dropdown.Item>
                                        </DropdownButton>
                                }
                                
                            </div>
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