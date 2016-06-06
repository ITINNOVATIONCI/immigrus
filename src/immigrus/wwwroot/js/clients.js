$(document).ready(function () {

$("#grid").kendoGrid({
    dataSource: {
        transport:
        {
            read: {
                url: "ListeClientss",
                dataType: "json"
            }
        },
        schema: {
            model: {
                id: "InscriptionId",
                fields: {
                    Id: {
                        //editable: false,
                        //nullable: false,
                        type: "string"
                    },
                    ConfimationNumber: {
                        type: "string",
                    },
                    Nom: {
                        //type: "date"
                        type:"string"
                    },
                    datenaisan: {
                        type: "string"
                    },
                    Tel1: {
                        type: "string"
                    },
                    Tel2: {
                        type: "string"
                    },
                    Email: {
                        type: "string"
                    },
                    //TypeTransfert: {
                    //    type: "string"
                    //},
                    //status: {
                    //    type: "string"
                    //},
                }
            }
        },
        autoSync: true,
        pageSize: 10,
        serverPaging: false,
        sort: { field: 'Nom', dir: 'asc' },
        serverSorting: false
    },
    height: 550,
    sortable: true,
    pageable: true,
    filterable: {
        mode: "row"
    },
    reorderable: true,
    resizable: true,
    columns: [{
        field: "InscriptionId",
        title: "ID",
        //format: "{0:dd-MM-yyyy hh:mm:ss}",
        width: "180px",
        attributes: {
            "class": "table-cell",
            style: "font-weight:bold; color:crimson"
        }
    },
    {
        
        field: "ConfimationNumber",
        title: "ConfimationNumber",
        width: 200,
        attributes: {
            "class": "table-cell",
            style: "font-weight:bold; color:darkslategray"
        },
        filterable: {
            cell: {
                showOperators: false
            }
        }
    },
            {
                field: "Nom",
                title: "Nom",
                width: 220,
                filterable: {
                    cell: {
                        showOperators: false
                    }
                }
            },
                    {
                        field: "datenaisan",
                        title:"date",
                        width: 80,
                        attributes: {
                            "class": "table-cell",
                            style: "color:dodgerblue"
                        },
                        filterable: {
                            cell: {
                                showOperators: false
                            }
                        }
                    },
    {
        field: "Tel1",
        title: "Contact",
        width: 120,
        filterable: {
            cell: {
                showOperators: false
            }
        }
    },
    {
        field: "Tel2",
        title: "Contact 2",
        width: 120,
        filterable: {
            cell: {
                showOperators: false
            }
        }
    },
    {
        field: "Email",
        title: "Email",
        width: 130,
        filterable: {
            cell: {
                showOperators: false
            }
        }
    }
    ]
});

//$("#grid").kendoGrid({
//    dataSource: {
//        transport:
//        {
//            read: {
//                url: "ListeClientss",
//                dataType: "json"
//            }
//        },
//        schema: {
//            model: {
//                id: "InscriptionId",
//                fields: {
//                    Id: {
//                        editable: false,
//                        nullable: false,
//                        type: "string"
//                    },
//                    ConfimationNumber: {
//                        type: "string",
//                    },
//                    Nom: {
//                        type: "date"
//                        type: "string"
//                    },
//                    datenaisan: {
//                        type: "string"
//                    },
//                    Tel1: {
//                        type: "string"
//                    },
//                    Tel2: {
//                        type: "string"
//                    },
//                    Email: {
//                        type: "string"
//                    },
//                    TypeTransfert: {
//                        type: "string"
//                    },
//                    status: {
//                        type: "string"
//                    },
//                }
//            }
//        },
//        autoSync: true,
//        pageSize: 10,
//        serverPaging: false,
//        sort: { field: 'Nom', dir: 'asc' },
//        serverSorting: false
//    },
//    height: 550,
//    sortable: true,
//    pageable: true,
//    filterable: {
//        mode: "row"
//    },
//    reorderable: true,
//    resizable: true,
//    columns: [{
//        field: "InscriptionId",
//        title: "ID",
//        format: "{0:dd-MM-yyyy hh:mm:ss}",
//        width: "180px",
//        attributes: {
//            "class": "table-cell",
//            style: "font-weight:bold; color:crimson"
//        }
//    },
//    {
//        template: "<img src='/#: Photo #' width='40' height='40' /> #: ConfimationNumber #"
//        ,
//        field: "ConfimationNumber",
//        title: "ConfimationNumber",
//        width: 200,
//        attributes: {
//            "class": "table-cell",
//            style: "font-weight:bold; color:darkslategray"
//        },
//        filterable: {
//            cell: {
//                showOperators: false
//            }
//        }
//    },
//            {
//                field: "Nom",
//                title: "Nom",
//                width: 220,
//                filterable: {
//                    cell: {
//                        showOperators: false
//                    }
//                }
//            },
//                    {
//                        field: "datenaisan",
//                        title: "date",
//                        width: 80,
//                        attributes: {
//                            "class": "table-cell",
//                            style: "font-weight:bold; color:dodgerblue"
//                        },
//                        filterable: {
//                            cell: {
//                                showOperators: false
//                            }
//                        }
//                    },
//    {
//        field: "Tel1",
//        title: "Contact",
//        width: 120,
//        filterable: {
//            cell: {
//                showOperators: false
//            }
//        }
//    },
//    {
//        field: "Tel2",
//        title: "Contact 2",
//        width: 120,
//        filterable: {
//            cell: {
//                showOperators: false
//            }
//        }
//    },
//    {
//        field: "Email",
//        title: "Email",
//        width: 130,
//        filterable: {
//            cell: {
//                showOperators: false
//            }
//        }
//    }
//    ]
//});


});