package be.kdg.stemtest.viewmodel

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import be.kdg.stemtest.model.entity.*
import be.kdg.stemtest.model.repositories.ResultRepository
import be.kdg.stemtest.model.repositories.StatementRepository
import javax.inject.Inject

class CPgResultViewModel @Inject constructor(private val resultRepository: ResultRepository,
                                             private val statementRepository: StatementRepository
) : ViewModel() {

    fun getResult(): LiveData<String> {
        return resultRepository.getCustomPartyGameResult()
    }

    fun getAnswerOptions(): LiveData<List<AnswerOption>> {
        return statementRepository.getAllAnswerOptions()
    }

    fun getAnswers(): LiveData<List<StudentAnswer>> {
        return resultRepository.getAnswers()
    }

    fun getStatements(): LiveData<List<Statement>> {
        return statementRepository.getAllStatements()
    }

    fun getCorrectAnswers(): LiveData<List<AnswerOption>> {
       return resultRepository.getCorrectAnswers()
    }
}
