#!/bin/bash

#
# Detect .meta files that do not have a corresponding asset.
#

RETCODE=0
IFS=$'\n'  # Allow spaces in filenames within if statements.

for FILENAME in $(find -maxdepth 3 -name '*.meta'); do
    if ! [[ -f "${FILENAME%.*}" ]] && ! [[ -d "${FILENAME%.*}" ]] ; then
        echo "${FILENAME}" "does not have a corresponding asset. Please remove this file."
        RETCODE=1
    fi
done

exit ${RETCODE}
