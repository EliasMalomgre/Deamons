package be.kdg.stemtest.viewmodel

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import be.kdg.stemtest.model.entity.*
import be.kdg.stemtest.model.repositories.ConnectionRepository
import be.kdg.stemtest.model.repositories.StatementRepository
import javax.inject.Inject

class DgStellingViewModel @Inject constructor(private val statementRepository: StatementRepository,
                                              private val connectionRepository: ConnectionRepository)
    : ViewModel() {

    fun getStatement(index:Int): LiveData<Statement> {
        return statementRepository.getStatement(index)
    }

    fun getIndex(): Int? {
        return statementRepository.getIndex()
    }

    fun getAnswerOptions(index:Int): LiveData<List<AnswerOption>> {
        return statementRepository.getAnswerOptions(index)
    }

    fun pushAnswer(argument:String?, answerId:Int, index: Int){
        statementRepository.pushAnswer(argument,answerId,index)
    }

    fun getGameType(): LiveData<GameSettings> {
       return connectionRepository.getGameSettings()
    }

    fun getDefinitions(): LiveData<List<Definition>> {
        return statementRepository.getAllDefinitions()
    }


}

