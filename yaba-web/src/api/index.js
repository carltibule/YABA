import { callExternalApi } from "./apiHelper";
import { getAllBookmarks, getBookmark, createNewBookmark, updateBookmark, deleteBookmark, deleteBookmarks, hideBookmarks } from "./v1/bookmarks";
import { getAllTags } from "./v1/tags";
import { getWebsiteMetaData } from "./v1/misc";

export {
    callExternalApi,
    getAllBookmarks,
    getBookmark,
    createNewBookmark,
    updateBookmark,
    getWebsiteMetaData,
    getAllTags,
    deleteBookmark,
    deleteBookmarks,
    hideBookmarks,
};