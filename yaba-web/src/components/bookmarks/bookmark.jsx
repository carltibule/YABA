import React from "react";
import { Row, Col, Form, Button } from "../external";
import DateTimeHelper from "../../utils/dateTimeHelper";
import { getHighlightedText } from "../../utils";
import { useNavigate } from "react-router-dom";
import { Tag } from "../../components";

export function Bookmark(props) {
    const navigate = useNavigate();

    const getTruncatedDescription = () => `${props.bookmark.description.substring(0, 97)}${props.bookmark.description.length >= 97 ? "..." : ""}`;
    const getHighlightedTitle = () => getHighlightedText(props.bookmark.title, props.searchString);
    const getHighlightedDescription = () => getHighlightedText(getTruncatedDescription(), props.searchString)

    return <div className="mb-3">
        <Row>
            <Col xs={1} className="d-flex justify-content-center align-items-center">
                <Form.Check onChange={(e) => {props.onBookmarkSelected(e.target.checked)}} checked={props.bookmark.isSelected} />
            </Col>
            <Col xs={11}>
                <a href={props.bookmark.url} target="_blank" style={{textDecoration: "none"}}>{ getHighlightedTitle() }</a>
                <div className="font-weight-normal">{ getHighlightedDescription()}</div>
                <div>
                    {
                        props.bookmark.tags.map((tag) => <Tag key={props.bookmark.id-tag.id} tag={tag} searchString={props.searchString}/>)
                    }
                </div>
                <div>
                    <span>{DateTimeHelper.getFriendlyDate(props.bookmark.createdOn)} | </span>
                    <span>
                        <Button variant="link" className="p-0 me-2" style={{textDecoration: "none"}} onClick={() => navigate(`/bookmarks/${props.bookmark.id}`)}>Edit</Button>
                        <Button variant="link" className="p-0 me-2" style={{textDecoration: "none"}} onClick={props.onHideClicked}>{props.bookmark.isHidden ? "Unhide" : "Hide"}</Button>
                        <Button variant="link" className="text-danger p-0 me-2" style={{textDecoration: "none"}} onClick={props.onDeleteClicked}>Delete</Button>
                    </span>
                </div>
            </Col>
        </Row>
    </div>
}