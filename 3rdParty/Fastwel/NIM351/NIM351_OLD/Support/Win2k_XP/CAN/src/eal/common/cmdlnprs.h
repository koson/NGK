/**
 *
 * NAME:    cmdlnprs.h
 *
 * DESCRIPTION: Simple string tokenizer and command line parser
 *
 *
 * AUTHOR:  Alexander Lokotkov
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2010 Fastwel Group.
 *      All rights reserved.
 ********************************************************/

#ifndef ___CMD_LINE_PARSER_H___
#define ___CMD_LINE_PARSER_H___

#include <eal/el.h>

#define LT_INVALID_POS      ((size_t)(-1))
#define LT_DEFAULT_DELIM    EAL_T(' ')
#define LT_EMPTY_DELIMETER  EAL_T(' ')

// Token object
typedef struct _F_LINE_TOKEN
{
  // the line to be tokenized
  F_STR       line;
  // left delimeter
  F_CHAR      left_delim;
  // start token position in the source line
  size_t      pos;
  // token len
  size_t      len;
}F_LINE_TOKEN, *PF_LINE_TOKEN;


#define F_LineTok_reset(lt)\
{\
  (lt)->left_delim = LT_EMPTY_DELIMETER;\
  (lt)->line = NULL;\
  (lt)->pos = LT_INVALID_POS;\
  (lt)->len = 0;\
}

#define F_LineTok_init(lt, ln)\
{\
  (lt)->left_delim = LT_EMPTY_DELIMETER;\
  (lt)->line = ln;\
  (lt)->pos = 0;\
  (lt)->len = 0;\
}

#define F_LineTok_isValid(lt) (NULL != (lt) && NULL != (lt)->line && 0 != (lt)->len && LT_INVALID_POS != (lt)->pos )

// Parser output variable type
typedef enum _F_LINE_PARSER_TYPE
{
  // dirty value
  LNP_UNDEFINED,
  // long
  LNP_LONG,
  // string
  LNP_STRING

}F_LINE_PARSER_TYPE;


#define LN_PARSER_VAR_STR_LEN   64
#define LN_PARSER_VAR_NAME_LEN  16

typedef struct _F_LINE_PARSER_VAR
{
  // Variable name, for instance if '-t 289' was gotten on input,
  // the name is 't'
  F_CHAR              name[LN_PARSER_VAR_NAME_LEN];

  // Variable value place-holder, for instance if '-t 289' was gotten on input,
  // the value is '289' type of LNP_LONG.
  union
  {
    // ulong value
    unsigned long val_ulong;
    // string value, will be cut off the LN_PARSER_VAR_STRLEN
    F_CHAR        val_string[LN_PARSER_VAR_STR_LEN];

  }                   value;

  // variable type
  F_LINE_PARSER_TYPE  type;

}F_LINE_PARSER_VAR, *PF_LINE_PARSER_VAR;


#define F_LineVar_init(lpv)\
{\
  (lpv)->name[0] = EAL_T('\0');\
  (lpv)->value.val_ulong = 0;\
  (lpv)->type = LNP_UNDEFINED;\
}

#define F_LineVar_name(lpv) (lpv)->name
#define F_LineVar_type(lpv) (lpv)->type
#define F_LineVar_getLong(lpv) (lpv)->value.val_ulong
#define F_LineVar_getString(lpv) (lpv)->value.val_string

#define F_LineVar_isEmpty(lpv) (LNP_UNDEFINED == (lpv)->type)


#ifdef __cplusplus
extern "C"
{
#endif
  
  //----------------------------------------------------------------------------------------------------
  /** 
   *  Function		 : bool F_LineTokenizer_next(PF_LINE_TOKEN pToken, const F_STR delims)
   *
   *  Parameters	 :  pToken (in/out)
   *                    Token object, containing an input string to be tokenized as well as
   *                    the result of each F_Line_nextToken() call.
   *
   *                  delims (in)
   *                    Contains delimeters separating tokens (for instance: EAL_T(" \t;-)"). 
   *                    If NULL, the function uses EAL_T(" \t") as a default delimeteres.
   *
   *  Return value :  true, if some token extracted from pToken->line
   *
   *  Description	 : Searches for tokens within pToken->line delimeted with characters
   *                 contained in delims.
   **/
  bool F_LineTokenizer_next(PF_LINE_TOKEN pToken, const F_STR delims);

  //----------------------------------------------------------------------------------------------------
  /** 
   *  Function		 : size_t F_LineTokenizer_copy( const PF_LINE_TOKEN pToken, 
   *                                              F_STR dst_buffer, 
   *                                              const size_t buffer_size, 
   *                                              bool strip_quotes)
   *
   *  Parameters	 :  pToken (in)
   *                    Token object
   *                  dst_buffer (in/out)
   *                    Destination buffer to which the pToken characters should be copied
   *                  buffer_size (in)
   *                    Destination buffer size
   *                  strip_quotes (in)
   *                    Instructs whether to ommit '\"' characters
   *
   *  Return value :  Number of characters copied to the dst_buffer or 0.
   *
   *  Description	 :  Copies characters of pToken to the dst_buffer appending the null character at the end.
   *                  If buffer_size < pToken->len + 1, (buffer_size - 1) characters will be copied.
   *
   *
   *
   **/
  size_t F_LineTokenizer_copy(const PF_LINE_TOKEN pToken, F_STR dst_buffer, size_t buffer_size, bool strip_quotes);

  //----------------------------------------------------------------------------------------------------
  /** 
   *  Function		 : bool F_LineParser_getVariable(PF_LINE_PARSER_VAR pVariable, 
   *                                               const PF_LINE_TOKEN pToken, 
   *                                               const F_STR value_delims)
   *
   *  Parameters	 :  pVariable (out)
   *                    Destination variable
   *                  pToken (in)
   *                    The token containing the variable name and value separated with
   *                    some delimeter
   *                  value_delims (in)
   *                    Array of delimeters
   *
   *  Return value :  true, if the variable information was obtained from pToken
   *
   *  Description	 :  Fills in the pVariable object with the name, type and value passed within a string that's
   *                  denoted by pToken.
   *
   *                  Examples:
   *                            1. Input token: -Aha Bill
   *                               Contents of pVariable on return: { EAL_T("Aha"), EAL_T("Bill"), LNP_STRING }
   *                            2. Input token: -BaseAddr:0x280
   *                               Contents of pVariable on return: { EAL_T("BaseAddr"), 0x280, LNP_LONG }
   *
   *
   **/
  bool   F_LineParser_getVariable(PF_LINE_PARSER_VAR pVariable, const PF_LINE_TOKEN pToken, const F_STR value_delims);

  //----------------------------------------------------------------------------------------------------
  /** 
   *  Function		 : bool   F_LineParser_copyVariable(PF_LINE_PARSER_VAR pDestination, const PF_LINE_PARSER_VAR pSource);
   *
   *  Parameters	 :  pDestination (in/out)
   *                    Destination variable
   *                  pSource (in)
   *                    Source variable
   *
   *  Return value :  true, if pSource was copied to pDestination
   *
   *  Description	 :  Copies pSource to pDestination
   *
   **/
  bool   F_LineParser_copyVariable(PF_LINE_PARSER_VAR pDestination, const PF_LINE_PARSER_VAR pSource);

  /**
   *    Test usage:
   *    F_STR test_string = EAL_T(" asss -t:acdef -k 0x280 -v 1234 --p:sdsds");
   *    F_STR tok_delims = EAL_T("-");
   *    F_STR value_delims = EAL_T(" :");
   *    F_LineTokenizer_test(test_string, tok_delims, value_delims);
   *
   */
  void F_LineParser_test(const F_STR test_string, const F_STR tokens_delimeters, const F_STR values_delimeters);

#ifdef __cplusplus
}
#endif

#endif